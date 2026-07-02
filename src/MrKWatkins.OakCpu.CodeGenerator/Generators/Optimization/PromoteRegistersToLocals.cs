using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using Parameter = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Parameter;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators.Optimization;

// Caches registers in locals for the duration of an instruction method, committing them back to their fields only
// before each return. For example CALL sets PC three times and SP twice; promotion turns those into register
// operations with a single store each on exit. The bus handler may observe stale internal register state during
// OnActionRequired, which the emulator contract permits; Address and Data are data members, not registers, so they
// are never promoted and the bus contract is unaffected.
//
// A register is only promoted when it is written at least twice at full width and never written via a half
// register, so the flags register (a half of AF) and byte-register arithmetic are never affected. Reads of a
// promoted register's halves are rewritten as shifts of the local. Registers whose accesses span a call other than
// the bus handler, e.g. HandleInterrupts, are not promoted as such calls can observe register state. Locals cannot
// survive a step emulator suspension, so this optimization is only valid for instruction emulator methods.
internal sealed class PromoteRegistersToLocals : MethodBodyOptimization
{
    protected override IReadOnlyList<StatementSyntax> Optimize(FileGeneratorContext context, IReadOnlyList<StatementSyntax> statements)
    {
        var registers = CreateRegistersByFieldName(context.GeneratorContext.Configuration);
        var analysis = Analyse(registers, statements);
        var promotions = CreatePromotions(analysis, statements);

        return promotions.Count == 0 ? statements : Transform(promotions, statements);
    }

    // Maps the field name of each promotable register, and of the halves of 16-bit pairs, to the register that
    // would be promoted for it.
    [Pure]
    private static IReadOnlyDictionary<string, Register> CreateRegistersByFieldName(Configuration configuration)
    {
        var byFieldName = new Dictionary<string, Register>(StringComparer.Ordinal);
        foreach (var register in configuration.Registers.Values.Where(register => register.Parent == null))
        {
            byFieldName.Add(register.FieldName, register);
            if (register.HighRegister != null)
            {
                byFieldName.Add(register.HighRegister.FieldName, register);
            }

            if (register.LowRegister != null)
            {
                byFieldName.Add(register.LowRegister.FieldName, register);
            }
        }

        return byFieldName;
    }

    [Pure]
    private static Dictionary<Register, RegisterUsage> Analyse(IReadOnlyDictionary<string, Register> registers, IReadOnlyList<StatementSyntax> statements)
    {
        var usages = new Dictionary<Register, RegisterUsage>();
        var walker = new AnalysisWalker(registers, usages);
        foreach (var statement in statements)
        {
            walker.Visit(statement);
        }

        return usages;
    }

    [Pure]
    private static IReadOnlyList<Promotion> CreatePromotions(Dictionary<Register, RegisterUsage> usages, IReadOnlyList<StatementSyntax> statements)
    {
        var promotable = usages
            .Where(usage => usage.Value is { FullWrites: >= 2, HalfWrites: 0, SpansBarrier: false })
            .Select(usage => usage.Key)
            .OrderBy(register => register.FieldName, StringComparer.Ordinal)
            .ToList();

        if (promotable.Count == 0)
        {
            return [];
        }

        var localNames = GetLocalNames(statements);
        var promotions = new List<Promotion>();
        foreach (var register in promotable)
        {
            var localName = register.FieldName.ToLowerInvariant();
            if (localNames.Contains(localName))
            {
                continue;
            }

            promotions.Add(new Promotion(register, localName, FindFirstAccessIndex(usages[register], statements)));
        }

        return promotions;
    }

    [Pure]
    private static HashSet<string> GetLocalNames(IReadOnlyList<StatementSyntax> statements) =>
        statements
            .SelectMany(statement => statement.DescendantNodesAndSelf().OfType<VariableDeclaratorSyntax>())
            .Select(declarator => declarator.Identifier.ValueText)
            .ToHashSet(StringComparer.Ordinal);

    [Pure]
    private static int FindFirstAccessIndex(RegisterUsage usage, IReadOnlyList<StatementSyntax> statements)
    {
        for (var index = 0; index < statements.Count; index++)
        {
            if (statements[index].DescendantNodesAndSelf().Any(node => usage.AccessNodes.Contains(node)))
            {
                return index;
            }
        }

        throw new InvalidOperationException("Promoted register has no accesses.");
    }

    [Pure]
    private static IReadOnlyList<StatementSyntax> Transform(IReadOnlyList<Promotion> promotions, IReadOnlyList<StatementSyntax> statements)
    {
        var rewriter = new AccessRewriter(promotions);
        var declared = new List<Promotion>();
        var result = new List<StatementSyntax>(statements.Count + (promotions.Count * 2));

        for (var index = 0; index < statements.Count; index++)
        {
            foreach (var promotion in promotions.Where(promotion => promotion.FirstAccessStatementIndex == index))
            {
                result.Add(CreateDeclaration(promotion));
                declared.Add(promotion);
            }

            result.Add(ProcessStatement((StatementSyntax)rewriter.Visit(statements[index]), declared, result));
        }

        return result;
    }

    // Inserts the field stores for the declared promoted registers before every return in the statement. Returns at
    // the top level have their stores added to the enclosing statement list instead.
    [MustUseReturnValue]
    private static StatementSyntax ProcessStatement(StatementSyntax statement, IReadOnlyList<Promotion> declared, List<StatementSyntax>? topLevel)
    {
        switch (statement)
        {
            case ReturnStatementSyntax when declared.Count > 0 && topLevel != null:
                topLevel.AddRange(declared.Select(CreateStore));
                return statement;

            case ReturnStatementSyntax when declared.Count > 0:
                return Block(List(declared.Select(CreateStore).Append(statement)));

            case BlockSyntax block:
                return block.WithStatements(List(ProcessBlockStatements(block.Statements, declared)));

            case IfStatementSyntax ifStatement:
                return ifStatement
                    .WithStatement(ProcessStatement(ifStatement.Statement, declared, null))
                    .WithElse(ifStatement.Else?.WithStatement(ProcessStatement(ifStatement.Else.Statement, declared, null)));

            default:
                return statement;
        }
    }

    [Pure]
    private static IEnumerable<StatementSyntax> ProcessBlockStatements(IEnumerable<StatementSyntax> statements, IReadOnlyList<Promotion> declared)
    {
        foreach (var statement in statements)
        {
            if (statement is ReturnStatementSyntax && declared.Count > 0)
            {
                foreach (var promotion in declared)
                {
                    yield return CreateStore(promotion);
                }

                yield return statement;
            }
            else
            {
                yield return ProcessStatement(statement, declared, null);
            }
        }
    }

    [Pure]
    private static StatementSyntax CreateDeclaration(Promotion promotion) =>
        LocalDeclarationStatement(
            VariableDeclaration(IdentifierName("var"))
                .WithVariables(
                [
                    VariableDeclarator(promotion.LocalName)
                        .WithInitializer(EqualsValueClause(EmulatorMemberIdentifier(promotion.Register.FieldName)))
                ]));

    [Pure]
    private static StatementSyntax CreateStore(Promotion promotion) =>
        ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                EmulatorMemberIdentifier(promotion.Register.FieldName),
                IdentifierName(promotion.LocalName)));

    [Pure]
    private static bool TryGetEmulatorField(ExpressionSyntax expression, [NotNullWhen(true)] out string? fieldName)
    {
        if (expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: Parameter.Name.Emulator },
                Name.Identifier.ValueText: var name
            })
        {
            fieldName = name;
            return true;
        }

        fieldName = null;
        return false;
    }

    private sealed record Promotion(Register Register, string LocalName, int FirstAccessStatementIndex);

    private sealed class RegisterUsage
    {
        public int FullWrites { get; set; }

        public int HalfWrites { get; set; }

        public bool SpansBarrier { get; set; }

        public HashSet<SyntaxNode> AccessNodes { get; } = [];
    }

    private sealed class AnalysisWalker(IReadOnlyDictionary<string, Register> registers, Dictionary<Register, RegisterUsage> usages) : CSharpSyntaxWalker
    {
        private bool insideReturn;

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            insideReturn = true;
            base.VisitReturnStatement(node);
            insideReturn = false;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // A call to anything other than the bus handler can legitimately observe register state, e.g.
            // HandleInterrupts or a dispatched instruction, so registers accessed before it cannot be promoted.
            // Registers accessed for the first time after the call get a fresh usage and are unaffected. Calls
            // inside a return are not barriers because promoted registers are always stored back before the return.
            if (!insideReturn && !IsBusHandlerCall(node) && !IsPureHelperCall(node))
            {
                foreach (var usage in usages.Values)
                {
                    usage.SpansBarrier = true;
                }
            }

            base.VisitInvocationExpression(node);
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (TryGetEmulatorField(node, out var fieldName) && registers.TryGetValue(fieldName, out var register))
            {
                if (!usages.TryGetValue(register, out var usage))
                {
                    usage = new RegisterUsage();
                    usages.Add(register, usage);
                }

                usage.AccessNodes.Add(node);
                if (node.Parent is AssignmentExpressionSyntax assignment && assignment.Left == node)
                {
                    if (fieldName == register.FieldName)
                    {
                        usage.FullWrites++;
                    }
                    else
                    {
                        usage.HalfWrites++;
                    }
                }
            }

            base.VisitMemberAccessExpression(node);
        }

        [Pure]
        private static bool IsBusHandlerCall(InvocationExpressionSyntax node) =>
            node.Expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: InstructionHandlerSyntax.ParameterName }
            };

        [Pure]
        private static bool IsPureHelperCall(InvocationExpressionSyntax node) =>
            node.Expression is MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax { Identifier.ValueText: "Unsafe" or "MemoryMarshal" }
            };
    }

    private sealed class AccessRewriter(IReadOnlyList<Promotion> promotions) : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (TryGetEmulatorField(node, out var fieldName))
            {
                foreach (var promotion in promotions)
                {
                    var register = promotion.Register;
                    if (fieldName == register.FieldName)
                    {
                        return IdentifierName(promotion.LocalName).WithTriviaFrom(node);
                    }

                    if (fieldName == register.HighRegister?.FieldName)
                    {
                        // emulator.PCH becomes (byte)(pc >> 8).
                        return CastExpression(
                                PredefinedType(Token(SyntaxKind.ByteKeyword)),
                                ParenthesizedExpression(
                                    BinaryExpression(
                                        SyntaxKind.RightShiftExpression,
                                        IdentifierName(promotion.LocalName),
                                        GenerateNumericLiteralExpression(8))))
                            .WithTriviaFrom(node);
                    }

                    if (fieldName == register.LowRegister?.FieldName)
                    {
                        // emulator.PCL becomes (byte)pc.
                        return CastExpression(PredefinedType(Token(SyntaxKind.ByteKeyword)), IdentifierName(promotion.LocalName)).WithTriviaFrom(node);
                    }
                }
            }

            return base.VisitMemberAccessExpression(node) ?? node;
        }
    }
}