using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class EmulatorClassGenerator : ClassGenerator
{
    protected const string AddressPropertyName = "Address";
    protected const string DataPropertyName = "Data";
    protected const string StepIndexFieldName = "stepIndex";
    protected const string LastOpcodeFieldName = "lastOpcode";

    private protected EmulatorClassGenerator()
    {
    }

    protected sealed override BaseTypeDeclarationSyntax CreateType(HashSet<string> requiredUsings, GeneratorInput input) =>
        PopulateClass(requiredUsings, input, SyntaxFactory.ClassDeclaration(EmulatorClassName).AddModifiers(Public, Sealed, Partial));

    [Pure]
    protected abstract ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration);

    [Pure]
    protected static StatementSyntax CreateSetMember(string member, string valueExpression) => CreateSetMember(member, SyntaxFactory.IdentifierName(valueExpression));

    [Pure]
    protected static StatementSyntax CreateSetMember(string member, ExpressionSyntax valueExpression) =>
        SyntaxFactory.ExpressionStatement(
            SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(member),
                valueExpression));
}