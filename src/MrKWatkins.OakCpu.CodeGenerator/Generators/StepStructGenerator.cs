using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class StepStructGenerator : TypeGenerator
{
    private const string StepHandlerParameterName = "handler";
    private const string StepNextStepParameterName = "nextStep";
    private const string StepActionRequiredParameterName = "actionRequired";

    public static readonly StepStructGenerator Instance = new();

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var actionType = NullableType(GenericName(Identifier(nameof(Action)))
            .WithTypeArgumentList(TypeArgumentList([IdentifierName(GetEmulatorClassName(context))])));

        var actionRequiredType = IdentifierName(ActionRequiredEnumName);

        return StructDeclaration(StepStructName)
            .WithModifiers(TokenList(Internal, ReadOnly))
            .WithParameterList(
                ParameterList(
                [
                    Parameter(Identifier(StepHandlerParameterName)).WithType(actionType),
                    Parameter(Identifier(StepNextStepParameterName)).WithType(UShort),
                    Parameter(Identifier(StepActionRequiredParameterName)).WithType(actionRequiredType)
                ]))
            .WithMembers(
            [
                CreateField(actionType, StepHandlerFieldName, StepHandlerParameterName),
                CreateField(UShort, StepNextStepFieldName, StepNextStepParameterName),
                CreateField(actionRequiredType, StepActionRequiredFieldName, StepActionRequiredParameterName)
            ]);
    }

    [Pure]
    private static FieldDeclarationSyntax CreateField(TypeSyntax type, string fieldName, string constructorParameterName) =>
        FieldDeclaration(VariableDeclaration(type).WithVariables([VariableDeclarator(Identifier(fieldName)).WithInitializer(EqualsValueClause(IdentifierName(constructorParameterName)))]))
            .WithModifiers([Internal, ReadOnly])
            .WithSemicolonToken(Semicolon);
}