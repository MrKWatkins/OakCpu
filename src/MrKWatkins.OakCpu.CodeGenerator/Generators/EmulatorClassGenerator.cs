using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public abstract class EmulatorClassGenerator : ClassGenerator
{
    private protected EmulatorClassGenerator()
    {
    }

    protected sealed override BaseTypeDeclarationSyntax CreateType(HashSet<string> requiredUsings, GeneratorInput input) =>
        PopulateClass(requiredUsings, input, SyntaxFactory.ClassDeclaration(GetEmulatorClassName(input)).AddModifiers(Public, Sealed, Partial));

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