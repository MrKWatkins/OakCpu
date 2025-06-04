using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Expressions.Ast;

public sealed class RegisterAccess(Register register) : Access(register.Name)
{
    public Register Register { get; } = register;

    public override Type Type => Register.Type;

    public override TypeSyntax TypeSyntax => Register.TypeSyntax;

    public override IdentifierNameSyntax Identifier => SyntaxFactory.IdentifierName(Register.FieldName);
}