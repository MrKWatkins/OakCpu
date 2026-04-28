using MrKWatkins.OakCpu.CodeGenerator.Definitions;
using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests;

public sealed class GeneratorContextTests : TestFixture
{
    [Test]
    public void Create()
    {
        var generatorInput = GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);
        generatorInput.Cpu.Name.Should().Equal("Z80");
        generatorInput.OpcodeRead.Should().HaveCount(4);
        generatorInput.OverlapSteps.Should().NotBeEmpty();
    }

    [Test]
    public void Create_DeduplicatesIdenticalOverlapHandlers()
    {
        var generatorInput = GeneratorContext.Create("MrKWatkins.OakCpu.Z80", Z80Yaml);

        var ldcc = generatorInput.Instructions.Single(i => i.Prefix == null && i.Opcode == 0x49);
        var lddd = generatorInput.Instructions.Single(i => i.Prefix == null && i.Opcode == 0x52);

        generatorInput.GetOverlapIndex(ldcc.Steps.Single()).Should().Equal(generatorInput.GetOverlapIndex(lddd.Steps.Single()));
    }

    [Test]
    public void GetImplicitInstructionCompleteStatementCount_ReturnsSuffixCount_WhenStepEndsWithOnInstructionComplete()
    {
        var step = Z80GeneratorContext.Instructions
            .SelectMany(instruction => instruction.Steps)
            .First(EndsWithOnInstructionComplete);

        Z80GeneratorContext.GetImplicitInstructionCompleteStatementCount(step).Should().Equal(Z80GeneratorContext.OnInstructionComplete.Count);
    }

    [Test]
    public void GetImplicitInstructionCompleteStatementCount_ReturnsZero_WhenStepDoesNotEndWithOnInstructionComplete()
    {
        var step = Z80GeneratorContext.Instructions
            .SelectMany(instruction => instruction.Steps)
            .First(step => !EndsWithOnInstructionComplete(step));

        Z80GeneratorContext.GetImplicitInstructionCompleteStatementCount(step).Should().Equal(0);
    }

    [Test]
    public void InstructionEmulatorSequences_IncludeOpcodeReadAndPrefixJumps()
    {
        Z80GeneratorContext.InstructionEmulatorSequences.Should().Contain(Z80GeneratorContext.OpcodeRead);
        foreach (var prefixJump in Z80GeneratorContext.PrefixJumps.Values)
        {
            Z80GeneratorContext.InstructionEmulatorSequences.Should().Contain(prefixJump);
        }
    }

    [Pure]
    private static bool EndsWithOnInstructionComplete(Step step)
    {
        var suffix = Z80GeneratorContext.OnInstructionComplete;
        return suffix.Count != 0 &&
               step.Statements.Count >= suffix.Count &&
               step.Statements
                   .Skip(step.Statements.Count - suffix.Count)
                   .Zip(suffix, (statement, suffixStatement) => ReferenceEquals(statement, suffixStatement))
                   .All(match => match);
    }
}