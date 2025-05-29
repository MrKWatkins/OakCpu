using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

        return ClassDeclaration(GetFlagsClassName(input))
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

        var flagsMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(flagsRegister.FieldName));

        var getExpression = BinaryExpression(
            SyntaxKind.NotEqualsExpression,
            ParenthesizedExpression(
                BinaryExpression(
                    SyntaxKind.BitwiseAndExpression,
                    flagsMemberAccess,
                    GetBinaryLiteralExpression(getMask))),
            GetNumericLiteralExpression(0));

        var setExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            flagsMemberAccess,
            CastExpression(
                flagsRegister.DataType.TypeSyntax(),
                ParenthesizedExpression(
                    ConditionalExpression(
                        IdentifierName("value"),
                        BinaryExpression(
                            SyntaxKind.BitwiseOrExpression,
                            flagsMemberAccess,
                            GetBinaryLiteralExpression(setMask)),
                        BinaryExpression(
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

        return ConstructorDeclaration(GetFlagsClassName(input))
            .WithModifiers(TokenList(Internal))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier(EmulatorFieldName))
                            .WithType(GetEmulatorClassIdentifier(input)))))
            .WithBody(Block(statements));
    }
}