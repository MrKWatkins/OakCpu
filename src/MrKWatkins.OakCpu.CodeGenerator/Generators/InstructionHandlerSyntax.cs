using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Provides the syntax fragments used to thread the generic bus-handler (<c>ref THandler</c>) through the generated
/// instruction emulator. The handler replaces the previous <c>Action&lt;ActionRequired, ushort, byte&gt;</c> delegate so the
/// JIT can monomorphise and inline the bus callback for each concrete handler struct.
/// </summary>
internal static class InstructionHandlerSyntax
{
    /// <summary>
    /// The name of the generic handler type parameter.
    /// </summary>
    public const string TypeParameterName = "THandler";

    /// <summary>
    /// The name of the handler parameter.
    /// </summary>
    public const string ParameterName = "handler";

    /// <summary>
    /// The name of the bus-action method declared on the handler interface.
    /// </summary>
    public const string OnActionRequiredMethodName = "OnActionRequired";

    /// <summary>
    /// The name of the nested generic class that holds the per-handler dispatch table.
    /// </summary>
    public const string DispatchHolderName = "Dispatch";

    /// <summary>
    /// Gets <c>Dispatch&lt;THandler&gt;</c>, the per-handler dispatch holder type.
    /// </summary>
    [Pure]
    public static TypeSyntax DispatchHolderType =>
        GenericName(Identifier(DispatchHolderName)).WithTypeArgumentList(TypeArguments);

    /// <summary>
    /// Gets the handler type, i.e. <c>THandler</c>.
    /// </summary>
    [Pure]
    public static TypeSyntax Type => IdentifierName(TypeParameterName);

    /// <summary>
    /// Gets the generic type parameter list, i.e. <c>&lt;THandler&gt;</c>.
    /// </summary>
    [Pure]
    public static TypeParameterListSyntax TypeParameters =>
        TypeParameterList(SingletonSeparatedList(TypeParameter(Identifier(TypeParameterName))));

    /// <summary>
    /// Gets the generic type argument list, i.e. <c>&lt;THandler&gt;</c>.
    /// </summary>
    [Pure]
    public static TypeArgumentListSyntax TypeArguments =>
        TypeArgumentList(SingletonSeparatedList(Type));

    /// <summary>
    /// Gets the <c>ref THandler handler</c> parameter.
    /// </summary>
    [Pure]
    public static ParameterSyntax MethodParameter =>
        Parameter(Identifier(ParameterName))
            .WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
            .WithType(Type);

    /// <summary>
    /// Gets the <c>ref handler</c> argument.
    /// </summary>
    [Pure]
    public static ArgumentSyntax Argument =>
        SyntaxFactory.Argument(IdentifierName(ParameterName))
            .WithRefKindKeyword(Token(SyntaxKind.RefKeyword));

    /// <summary>
    /// Gets the <c>where THandler : I{Cpu}BusHandler, allows ref struct</c> constraint clauses.
    /// </summary>
    [Pure]
    public static SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses(GeneratorContext context) =>
        SingletonList(
            TypeParameterConstraintClause(IdentifierName(TypeParameterName))
                .WithConstraints(
                    SeparatedList<TypeParameterConstraintSyntax>(
                    [
                        TypeConstraint(IdentifierName(Identifiers.Class.Name.InstructionBusHandler(context))),
                        AllowsConstraintClause(SingletonSeparatedList<AllowsConstraintSyntax>(RefStructConstraint()))
                    ])));

    /// <summary>
    /// Creates the <c>delegate*&lt;{Emulator}, ref THandler, int&gt;</c> instruction handler type.
    /// </summary>
    [Pure]
    public static FunctionPointerTypeSyntax InstructionHandlerType(GeneratorContext context) =>
        FunctionPointerType(
            null,
            FunctionPointerParameterList(
            [
                FunctionPointerParameter(IdentifierName(Identifiers.Class.Name.InstructionEmulator(context))),
                FunctionPointerParameter(Type).WithModifiers(TokenList(Token(SyntaxKind.RefKeyword))),
                FunctionPointerParameter(PredefinedType(Token(SyntaxKind.IntKeyword)))
            ]));

    /// <summary>
    /// Creates <c>&amp;{methodName}&lt;THandler&gt;</c>, the address of a generic instruction method instantiated at the
    /// handler type.
    /// </summary>
    [Pure]
    public static ExpressionSyntax AddressOfInstructionMethod(string methodName) =>
        PrefixUnaryExpression(
            SyntaxKind.AddressOfExpression,
            GenericName(Identifier(methodName)).WithTypeArgumentList(TypeArguments));
}