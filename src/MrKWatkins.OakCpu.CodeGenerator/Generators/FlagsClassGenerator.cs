using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

public sealed class FlagsClassGenerator : TypeGenerator
{
    public static readonly FlagsClassGenerator Instance = new();

    private FlagsClassGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => GetFlagsClassName(context);

    [Pure]
    public override IReadOnlyList<GeneratedFile> GenerateFiles(GeneratorContext context) => GenerateOneFilePerType(context);

    protected override IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(GeneratorContext context)
    {
        yield return CreateBaseClass(context);
        yield return CreateConcreteClass(context, GetStepFlagsClassName(context), GetEmulatorClassIdentifier(context));
        yield return CreateConcreteClass(context, GetInstructionFlagsClassName(context), GetInstructionEmulatorClassIdentifier(context));
    }

    [Pure]
    private static ClassDeclarationSyntax CreateBaseClass(GeneratorContext context)
    {
        var members = CreateFlagProperties(context, createOverrideProperty: false).Cast<MemberDeclarationSyntax>().ToArray();

        return WithXmlDocumentation(
            ClassDeclaration(GetFlagsClassName(context))
                .AddModifiers(Public, Abstract)
                .AddMembers(members),
            $"Provides access to the {context.Cpu.Name} flags.");
    }

    [Pure]
    private static ClassDeclarationSyntax CreateConcreteClass(GeneratorContext context, string className, TypeSyntax emulatorType)
    {
        var members = new List<MemberDeclarationSyntax>
        {
            CreateEmulatorField(emulatorType),
            CreateConstructor(className, emulatorType)
        };
        members.AddRange(CreateFlagProperties(context, createOverrideProperty: true));

        return ClassDeclaration(className)
            .AddModifiers(Internal, Sealed)
            .WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(SimpleBaseType(IdentifierName(GetFlagsClassName(context))))))
            .AddMembers(members.ToArray());
    }

    [Pure]
    private static IEnumerable<PropertyDeclarationSyntax> CreateFlagProperties(GeneratorContext context, bool createOverrideProperty) =>
        context.Configuration.Flags.Values.Select(flag => CreateFlagProperty(context, flag, createOverrideProperty));

    [Pure]
    private static PropertyDeclarationSyntax CreateFlagProperty(GeneratorContext context, Flag flag, bool createOverrideProperty)
    {
        if (!createOverrideProperty)
        {
            return WithXmlDocumentation(CreateAbstractGetSetProperty(BoolType, flag.Name), flag.Documentation);
        }

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

        return CreateOverrideGetSetProperty(context, BoolType, flag.Name, getExpression, setExpression);
    }

    [Pure]
    private static ConstructorDeclarationSyntax CreateConstructor(string className, TypeSyntax emulatorType) =>
        ConstructorDeclaration(className)
            .WithModifiers(TokenList(Internal))
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier(EmulatorFieldName))
                            .WithType(emulatorType))))
            .WithBody(Block(CreateAssignEmulatorFieldExpression()));
}