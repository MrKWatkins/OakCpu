using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class FlagsClassGenerator : ClassGenerator
{
    public static readonly FlagsClassGenerator Instance = new();

    private FlagsClassGenerator()
    {
    }

    protected override BaseTypeDeclarationSyntax CreateType(HashSet<string> requiredUsings, GeneratorInput input)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(input),
            CreateConstructor(input)
        };

        members.AddRange(CreateFlagProperties(input));

        return SyntaxFactory
            .ClassDeclaration(GetFlagsClassName(input))
            .AddModifiers(Public, Sealed)
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateFlagProperties(GeneratorInput input) => input.Flags.Select(f => CreateFlagProperty(input.FlagsRegister, f));

    [Pure]
    private static PropertyDeclarationSyntax CreateFlagProperty(Register flagsRegister, Flag flag)
    {
        var getMask = (byte)(1 << flag.Index);
        var setMask = (byte)(1 << flag.Index);
        var resetMask = (byte)~(1 << flag.Index);

        var flagsMemberAccess = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName(EmulatorFieldName),
            SyntaxFactory.IdentifierName(flagsRegister.FieldName));

        var getExpression = SyntaxFactory.BinaryExpression(
            SyntaxKind.NotEqualsExpression,
            SyntaxFactory.ParenthesizedExpression(
                SyntaxFactory.BinaryExpression(
                    SyntaxKind.BitwiseAndExpression,
                    flagsMemberAccess,
                    GetBinaryLiteralExpression(getMask))),
            GetNumericLiteralExpression(0));

        var setExpression = SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            flagsMemberAccess,
            SyntaxFactory.CastExpression(
                flagsRegister.Type.PredefinedType(),
                SyntaxFactory.ParenthesizedExpression(
                    SyntaxFactory.ConditionalExpression(
                        SyntaxFactory.IdentifierName("value"),
                        SyntaxFactory.BinaryExpression(
                            SyntaxKind.BitwiseOrExpression,
                            flagsMemberAccess,
                            GetBinaryLiteralExpression(setMask)),
                        SyntaxFactory.BinaryExpression(
                            SyntaxKind.BitwiseAndExpression,
                            flagsMemberAccess,
                            GetBinaryLiteralExpression(resetMask))))));

        return CreateGetSetProperty(Bool, flag.Name, getExpression, setExpression);
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorInput input)
    {
        var statements = new StatementSyntax[]
        {
            CreateAssignEmulatorFieldExpression()
        };

        return SyntaxFactory
            .ConstructorDeclaration(GetFlagsClassName(input))
            .WithModifiers(SyntaxFactory.TokenList(Internal))
            .WithParameterList(
                SyntaxFactory.ParameterList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory
                            .Parameter(SyntaxFactory.Identifier(EmulatorFieldName))
                            .WithType(GetEmulatorClassIdentifier(input)))))
            .WithBody(SyntaxFactory.Block(statements));
    }
}