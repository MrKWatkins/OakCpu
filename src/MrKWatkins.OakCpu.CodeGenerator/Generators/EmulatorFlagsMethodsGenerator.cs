using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags;
using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorFlagsMethodsGenerator : EmulatorClassGenerator
{
    public static readonly EmulatorFlagsMethodsGenerator Instance = new();

    private EmulatorFlagsMethodsGenerator()
    {
    }

    protected override ClassDeclarationSyntax PopulateClass(GeneratorContext context, ClassDeclarationSyntax classDeclaration) =>
        classDeclaration.AddMembers(CreateFlagsMethods(context).ToArray());

    [Pure]
    private static IEnumerable<MemberDeclarationSyntax> CreateFlagsMethods(GeneratorContext context) =>
        context.Instructions.Where(i => i.Flags.Any()).Select(i => CreateFlagsMethod(context, i));

    [Pure]
    private static MemberDeclarationSyntax CreateFlagsMethod(GeneratorContext context, Instruction instruction)
    {
        var step = instruction.Steps.FirstOrDefault(step => step.Statements.Any(statement => statement is CallStatement call && call.Call.Function == PreDefinedFunction.Flags));
        if (step == null)
        {
            throw new InvalidOperationException($"Instruction {instruction.Mnemonic} has a flags section but no step with a flags() call.");
        }

        var stepContext = new StepContext(context, step);
        var temporaryVariables = instruction.TemporaryVariablesUsedByFlags.ToList();
        foreach (var temporary in temporaryVariables)
        {
            stepContext.InitializedTemporaryVariables.Add(temporary);
        }

        // Tried adding both data members and registers as parameters to the methods when they were used more than once, in case using a local
        // variable is cheaper than repeated field access. However, the disassembly only changed by one argument to the initial MOV and there
        // was no change in performance.
        var parameters = temporaryVariables
            .Select(t => Parameter(Identifier(t)).WithType(PredefinedType(Token(SyntaxKind.IntKeyword))))
            .Prepend(CreateEmulatorParameter(context))
            .ToArray();

        // Tried returning the flags value and assigning to the field in the steps. No noticeable change in performance but it does increase the
        // size of the step method and we want to keep that as small as possible.
        return MethodDeclaration(
                Void,
                Identifier(GetFlagsMethodName(step)))
            .AddModifiers(Private, Static)
            .AddParameterListParameters(parameters)
            .WithAttributeLists([AttributeList([CreateMethodImplAttribute(context, "AggressiveOptimization")])])    // TODO: Inline
            .WithBody(Block(FlagsGenerator.GenerateFlagsStatements(stepContext)))
            .WithLeadingTrivia(Comment($"// {step.Name}"));
    }
}