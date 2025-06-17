using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class FlagsClassGenerator : TypeGenerator
{
    public static readonly FlagsClassGenerator Instance = new();

    private FlagsClassGenerator()
    {
    }

    protected override BaseTypeDeclarationSyntax CreateType(GeneratorContext context)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(context),
            CreateConstructor(context)
        };

        members.AddRange(CreateFlagProperties(context));

        return ClassDeclaration(GetFlagsClassName(context))
            .AddModifiers(Public, Sealed)
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateFlagProperties(GeneratorContext context) => context.Configuration.Flags.Values.Select(f => CreateFlagProperty(context, f));

    [Pure]
    private static PropertyDeclarationSyntax CreateFlagProperty(GeneratorContext context, Flag flag)
    {
        var getMask = (byte)(1 << flag.Index);
        var setMask = (byte)(1 << flag.Index);
        var resetMask = (byte)~(1 << flag.Index);

        var flagsMemberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(EmulatorFieldName),
            IdentifierName(context.Configuration.FlagsRegister.FieldName));

        var getExpression = BinaryExpression(
            SyntaxKind.NotEqualsExpression,
            ParenthesizedExpression(
                BinaryExpression(
                    SyntaxKind.BitwiseAndExpression,
                    flagsMemberAccess,
                    GenerateBinaryLiteralExpression(getMask))),
            GenerateNumericLiteralExpression(0));

        var setExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            flagsMemberAccess,
            CastExpression(
                context.Configuration.FlagsRegister.Type.TypeSyntax(),
                ParenthesizedExpression(
                    ConditionalExpression(
                        IdentifierName("value"),
                        BinaryExpression(
                            SyntaxKind.BitwiseOrExpression,
                            flagsMemberAccess,
                            GenerateBinaryLiteralExpression(setMask)),
                        BinaryExpression(
                            SyntaxKind.BitwiseAndExpression,
                            flagsMemberAccess,
                            GenerateBinaryLiteralExpression(resetMask))))));

        return CreateGetSetProperty(context, Bool, flag.Name, getExpression, setExpression);
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(GeneratorContext context)
    {
        var statements = new StatementSyntax[]
        {
            CreateAssignEmulatorFieldExpression()
        };

        return ConstructorDeclaration(GetFlagsClassName(context))
            .WithModifiers(TokenList(Internal))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier(EmulatorFieldName))
                            .WithType(GetEmulatorClassIdentifier(context)))))
            .WithBody(Block(statements));
    }
}