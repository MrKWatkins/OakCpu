using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static MrKWatkins.OakCpu.CodeGenerator.CommonSyntax;
using static MrKWatkins.OakCpu.CodeGenerator.Generators.Identifiers;

namespace MrKWatkins.OakCpu.CodeGenerator.Generators;

/// <summary>
/// Generates the flags facade classes for both emulator variants.
/// </summary>
public sealed class FlagsClassGenerator : TypeGenerator
{
    public static readonly FlagsClassGenerator Instance = new();

    private FlagsClassGenerator()
    {
    }

    protected override string GetBaseFileName(GeneratorContext context) => Class.Name.Flags(context);

    [Pure]
    public override IReadOnlyList<GeneratedFile> GenerateFiles(GeneratorContext context) => GenerateOneFilePerType(context);

    protected override IEnumerable<BaseTypeDeclarationSyntax> CreateTypes(FileGeneratorContext context)
    {
        yield return CreateBaseClass(context);
        yield return CreateConcreteClass(context, Class.Name.StepFlags(context), Class.Identifier.Emulator(context));
        yield return CreateConcreteClass(context, Class.Name.InstructionFlags(context), Class.Identifier.InstructionEmulator(context));
    }

    [Pure]
    private static ClassDeclarationSyntax CreateBaseClass(FileGeneratorContext context)
    {
        var members = CreateFlagProperties(context, createOverrideProperty: false).Cast<MemberDeclarationSyntax>().ToArray();

        return CreateFacadeBaseClass(Class.Name.Flags(context), $"Provides access to the {context.GeneratorContext.Cpu.Name} flags.", members);
    }

    [MustUseReturnValue]
    private static ClassDeclarationSyntax CreateConcreteClass(FileGeneratorContext context, string className, TypeSyntax emulatorType)
    {
        var members = CreateFlagProperties(context, createOverrideProperty: true).Cast<MemberDeclarationSyntax>().ToArray();
        return CreateFacadeConcreteClass(
            className,
            Class.Name.Flags(context),
            emulatorType,
            CreateFacadeConstructor(className, emulatorType),
            members);
    }

    [MustUseReturnValue]
    private static IEnumerable<PropertyDeclarationSyntax> CreateFlagProperties(FileGeneratorContext context, bool createOverrideProperty) =>
        context.GeneratorContext.Configuration.Flags.Values.Select(flag => CreateFlagProperty(context, flag, createOverrideProperty));

    [MustUseReturnValue]
    private static PropertyDeclarationSyntax CreateFlagProperty(FileGeneratorContext context, Flag flag, bool createOverrideProperty)
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
            IdentifierName(context.GeneratorContext.Configuration.FlagsRegister.FieldName));

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
                context.GeneratorContext.Configuration.FlagsRegister.Type.TypeSyntax(),
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

}