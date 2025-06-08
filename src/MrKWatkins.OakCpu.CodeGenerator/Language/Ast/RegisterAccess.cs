using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class RegisterAccess(Register register) : Access(register.Name)
{
    public Register Register { get; } = register;

    public override DataType Type => Register.Type;

    public override IdentifierNameSyntax Identifier => SyntaxFactory.IdentifierName(Register.FieldName);
}