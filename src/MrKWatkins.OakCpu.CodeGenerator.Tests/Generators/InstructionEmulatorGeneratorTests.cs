using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class InstructionEmulatorGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => InstructionEmulatorGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void Generate_EmitsCanonicalAndDisambiguatedInstructionMethods()
    {
        var result = InstructionEmulatorGenerator.Instance.Generate(Z80GeneratorContext);

        result.Contains("private static int NOP(", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("private static int NOP_DD_00(", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("private static int Prefix_CB(", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("private static int Error(", StringComparison.Ordinal).Should().BeTrue();
    }

    [Test]
    public void GetInstructionMethodName_ReturnsUniqueNamesForDispatchableSequences()
    {
        var names = InstructionEmulatorGenerator.GetDispatchableSequences(Z80GeneratorContext)
            .Select(sequence => InstructionEmulatorGenerator.GetInstructionMethodName(Z80GeneratorContext, sequence))
            .ToList();

        names.Distinct(StringComparer.Ordinal).Count().Should().Equal(names.Count);
    }

    [Test]
    public void GetInstructionMethodName_UsesUnsuffixedCanonicalNameForNoPrefixInstruction()
    {
        var instruction = Z80GeneratorContext.Instructions.Single(i => i is { Mnemonic: "NOP", Prefix: null, OpcodeTable: null });

        var name = InstructionEmulatorGenerator.GetInstructionMethodName(Z80GeneratorContext, instruction);

        name.Should().Equal("NOP");
    }

    [Test]
    public void GetInstructionMethodName_UsesEncodingSuffixForCollidingInstruction()
    {
        var instruction = Z80GeneratorContext.Instructions.Single(i => i.Mnemonic == "NOP" && i.Prefix == 0xDD);

        var name = InstructionEmulatorGenerator.GetInstructionMethodName(Z80GeneratorContext, instruction);

        name.Should().Equal("NOP_DD_00");
    }

    [Test]
    public void GetInstructionMethodName_UsesPrefixNameForPrefixJump()
    {
        var prefixJump = Z80GeneratorContext.PrefixJumps[0xCB];

        var name = InstructionEmulatorGenerator.GetInstructionMethodName(Z80GeneratorContext, prefixJump);

        name.Should().Equal("Prefix_CB");
    }
}