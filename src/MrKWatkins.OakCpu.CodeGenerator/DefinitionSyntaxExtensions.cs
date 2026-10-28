using System.Collections.Frozen;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.Definitions.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator;

/// <summary>
/// Roslyn syntax for the CPU definitions model, which is itself independent of the C# code generation.
/// </summary>
[SuppressMessage("Naming", "CA1708", Justification = "False positive; the extension blocks are not members.")]
public static class DefinitionSyntaxExtensions
{
    private static readonly FrozenDictionary<Operator, SyntaxKind> OperatorSyntaxKinds = new Dictionary<Operator, SyntaxKind>
    {
        [Operator.Not] = SyntaxKind.LogicalNotExpression,
        [Operator.BitwiseNot] = SyntaxKind.BitwiseNotExpression,
        [Operator.Multiply] = SyntaxKind.MultiplyExpression,
        [Operator.Add] = SyntaxKind.AddExpression,
        [Operator.Subtract] = SyntaxKind.SubtractExpression,
        [Operator.LeftShift] = SyntaxKind.LeftShiftExpression,
        [Operator.RightShift] = SyntaxKind.RightShiftExpression,
        [Operator.LessThan] = SyntaxKind.LessThanExpression,
        [Operator.LessThanOrEqual] = SyntaxKind.LessThanOrEqualExpression,
        [Operator.GreaterThan] = SyntaxKind.GreaterThanExpression,
        [Operator.GreaterThanOrEqual] = SyntaxKind.GreaterThanOrEqualExpression,
        [Operator.Equality] = SyntaxKind.EqualsExpression,
        [Operator.NotEquals] = SyntaxKind.NotEqualsExpression,
        [Operator.And] = SyntaxKind.BitwiseAndExpression,
        [Operator.Xor] = SyntaxKind.ExclusiveOrExpression,
        [Operator.Or] = SyntaxKind.BitwiseOrExpression,
        [Operator.LogicalAnd] = SyntaxKind.LogicalAndExpression,
        [Operator.LogicalOr] = SyntaxKind.LogicalOrExpression,
        [Operator.Assignment] = SyntaxKind.SimpleAssignmentExpression
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<Operator, SyntaxKind> CompoundAssignmentSyntaxKinds = new Dictionary<Operator, SyntaxKind>
    {
        [Operator.Multiply] = SyntaxKind.MultiplyAssignmentExpression,
        [Operator.Add] = SyntaxKind.AddAssignmentExpression,
        [Operator.Subtract] = SyntaxKind.SubtractAssignmentExpression,
        [Operator.LeftShift] = SyntaxKind.LeftShiftAssignmentExpression,
        [Operator.RightShift] = SyntaxKind.RightShiftAssignmentExpression,
        [Operator.And] = SyntaxKind.AndAssignmentExpression,
        [Operator.Xor] = SyntaxKind.ExclusiveOrAssignmentExpression,
        [Operator.Or] = SyntaxKind.OrAssignmentExpression
    }.ToFrozenDictionary();

    extension(Operator @operator)
    {
        /// <summary>
        /// Gets the <see cref="Microsoft.CodeAnalysis.CSharp.SyntaxKind" /> of the C# expression for the operator.
        /// </summary>
        public SyntaxKind SyntaxKind => OperatorSyntaxKinds[@operator];

        /// <summary>
        /// Gets the <see cref="Microsoft.CodeAnalysis.CSharp.SyntaxKind" /> of the C# compound assignment expression for the
        /// operator, or <c>null</c> if the operator has no compound assignment form.
        /// </summary>
        public SyntaxKind? CompoundAssignmentSyntaxKind => CompoundAssignmentSyntaxKinds.TryGetValue(@operator, out var kind) ? kind : null;
    }

    extension(Expression expression)
    {
        /// <summary>
        /// Gets the C# type syntax for the expression's type.
        /// </summary>
        public TypeSyntax TypeSyntax => expression.Type.TypeSyntax();
    }

    extension(Access access)
    {
        /// <summary>
        /// Gets the identifier of the generated emulator member the access refers to.
        /// </summary>
        public IdentifierNameSyntax Identifier => IdentifierName(
            access switch
            {
                RegisterAccess registerAccess => registerAccess.Register.FieldName,
                DataMemberAccess dataMemberAccess => dataMemberAccess.DataMember.FieldName,
                _ => access.Name
            });
    }

    extension(Register register)
    {
        /// <summary>
        /// Gets the C# type syntax for the register.
        /// </summary>
        public TypeSyntax TypeSyntax => register.Type.TypeSyntax();
    }

    extension(Function function)
    {
        /// <summary>
        /// Gets the C# type syntax for the function's return type.
        /// </summary>
        public TypeSyntax TypeSyntax => function.Type.TypeSyntax();
    }

    extension(DataMember dataMember)
    {
        /// <summary>
        /// Gets the C# type syntax for the data member.
        /// </summary>
        public TypeSyntax TypeSyntax => dataMember.Type.TypeSyntax(dataMember.IsArray);
    }
}