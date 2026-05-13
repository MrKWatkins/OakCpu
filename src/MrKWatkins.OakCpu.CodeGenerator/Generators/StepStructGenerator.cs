using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;
using Field = MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers.Field;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StepStructGenerator : TypeGenerator
{
    private const string StepHandlerParameterName = "handler";
    private const string StepNextStepParameterName = "nextStep";
    private const string StepActionRequiredParameterName = "actionRequired";
    private const string StepOverlapParameterName = "overlap";

    public static readonly StepStructGenerator Instance = new();

    private StepStructGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => TypeName.StepStruct;

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var actionType = FunctionPointerType(
            null,
            FunctionPointerParameterList(
                [
                    FunctionPointerParameter(IdentifierName(Class.Name.Emulator(context))),
                    FunctionPointerParameter(IdentifierName(TypeName.ActionRequiredEnum)).WithModifiers([Ref]),
                    FunctionPointerParameter(VoidType)
                 ]));

        var actionRequiredType = IdentifierName(TypeName.ActionRequiredEnum);
        var overlapType = CreateOverlapHandlerType(context);

        return StructDeclaration(TypeName.StepStruct)
            .WithModifiers(TokenList(Internal, Unsafe, ReadOnly))
            .WithParameterList(
                ParameterList(
                [
                    Parameter(Identifier(StepHandlerParameterName)).WithType(actionType),
                    Parameter(Identifier(StepNextStepParameterName)).WithType(UShortType),
                    Parameter(Identifier(StepActionRequiredParameterName)).WithType(actionRequiredType),
                    Parameter(Identifier(StepOverlapParameterName)).WithType(overlapType)
                ]))
            .WithMembers(
            [
                CreateField(actionType, Field.Name.Handler, StepHandlerParameterName),
                CreateField(UShortType, Field.Name.NextStep, StepNextStepParameterName),
                CreateField(actionRequiredType, Field.Name.ActionRequired, StepActionRequiredParameterName),
                CreateField(overlapType, Field.Name.Overlap, StepOverlapParameterName)
            ]);
    }

    [Pure]
    private static FieldDeclarationSyntax CreateField(TypeSyntax type, string fieldName, string constructorParameterName) =>
        FieldDeclaration(VariableDeclaration(type).WithVariables([VariableDeclarator(Identifier(fieldName)).WithInitializer(EqualsValueClause(IdentifierName(constructorParameterName)))]))
            .WithModifiers([Internal, ReadOnly])
            .WithSemicolonToken(Semicolon);
}