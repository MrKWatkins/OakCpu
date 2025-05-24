using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class EmulatorFieldsAndConstructor : EmulatorClassGenerator
{
    public static readonly EmulatorFieldsAndConstructor Instance = new();

    private EmulatorFieldsAndConstructor()
    {
    }

    protected override ClassDeclarationSyntax PopulateClass(HashSet<string> requiredUsings, GeneratorInput input, ClassDeclarationSyntax classDeclaration)
    {
        var structLayout = CreateStructLayoutAttribute(requiredUsings);

        return classDeclaration
            .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(structLayout)))
            .AddMembers(input.Registers.Select(r => CreateField(requiredUsings, r)).ToArray<MemberDeclarationSyntax>());
    }

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(HashSet<string> requiredUsings, Register register) =>
        CreateField(requiredUsings, register.FieldName, register.Type.PredefinedType(), register.FieldOffset, Internal);

    [MustUseReturnValue]
    private static FieldDeclarationSyntax CreateField(HashSet<string> requiredUsings, string name, PredefinedTypeSyntax type, int fieldOffset, SyntaxToken visibility)
    {
        var attribute = CreateFieldOffsetAttribute(requiredUsings, fieldOffset);

        return SyntaxFactory.FieldDeclaration(
            SyntaxFactory.VariableDeclaration(type)
                .WithVariables(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(name)))))
            .WithModifiers(SyntaxFactory.TokenList(visibility))
            .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(attribute)));
    }

    [MustUseReturnValue]
    private static AttributeSyntax CreateStructLayoutAttribute(HashSet<string> requiredUsings)
    {
        requiredUsings.Add("System.Runtime.InteropServices");

        return SyntaxFactory.Attribute(
            SyntaxFactory.IdentifierName("StructLayout"),
            SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.AttributeArgument(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("LayoutKind"),
                            SyntaxFactory.IdentifierName("Explicit"))))));
    }
    [MustUseReturnValue]
    private static AttributeSyntax CreateFieldOffsetAttribute(HashSet<string> requiredUsings, int fieldOffset)
    {
        requiredUsings.Add("System.Runtime.InteropServices");

        return SyntaxFactory.Attribute(
            SyntaxFactory.IdentifierName("FieldOffset"),
            SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.AttributeArgument(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.NumericLiteralExpression,
                            SyntaxFactory.Literal(fieldOffset))))));
    }
}