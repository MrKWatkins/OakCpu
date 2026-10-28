namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class CpuDocumentationTests : TestFixture
{
    [Test]
    public void Create_DocumentsEveryInstruction()
    {
        var forms = Z80Model.Yaml.Instructions
            .Where(yaml => yaml.Group != "Redirects")
            .Select(yaml => yaml.Instruction)
            .Distinct()
            .Order(StringComparer.Ordinal);

        Z80Documentation.Select(d => d.Instruction).Order(StringComparer.Ordinal).Should().SequenceEqual(forms);
    }

    [Test]
    public void Create_DoesNotDocumentRedirects() => Z80Documentation.Any(d => d.Instruction.Contains("Redirect", StringComparison.Ordinal)).Should().BeFalse();

    [Test]
    public void Create_OrdersByGroupThenInstruction()
    {
        var keys = Z80Documentation.Select(d => (d.Group, d.Instruction)).ToList();

        keys.Should().SequenceEqual(keys.OrderBy(k => k.Group, StringComparer.Ordinal).ThenBy(k => k.Instruction, StringComparer.Ordinal));
    }

    [Test]
    public void Create_PagesAreUnique() => Z80NDocumentation.Select(d => d.Page).Distinct(StringComparer.Ordinal).Count().Should().Equal(Z80NDocumentation.Count);

    [Test]
    public void Create_EveryInstructionHasDocumentation()
    {
        foreach (var documentation in Z80NDocumentation)
        {
            documentation.Documentation.Should().NotBeEmpty();
        }
    }

    [Test]
    public void Create_EveryFlagEffectIsDescribed()
    {
        var fallbacks = Z80NDocumentation.Where(d => d.FlagFallbacks.Count > 0).Select(d => $"{d.Instruction}: {string.Join(", ", d.FlagFallbacks)}").ToList();

        fallbacks.Should().BeEmpty();
    }

    [Test]
    public void Create_Load()
    {
        var documentation = GetDocumentation(Z80Documentation, "LD R0, (RP0 + d)");

        documentation.Cpu.Should().Equal("Z80");
        documentation.DefinedBy.Should().Equal("Z80");
        documentation.Symbolic.Should().Equal("LD r, (rr + d)");
        documentation.Group.Should().Equal("8-Bit Load");
        documentation.Undocumented.Should().BeFalse();
        documentation.Page.Should().Equal("8-bit-load/ld-r0-at-rp0-d.md");
        documentation.Flags.Should().OnlyContain(f => f.Kind == FlagEffectKind.Unaffected);
        documentation.Opcodes.Should().HaveCount(14);
        documentation.Opcodes[0].Opcode.Should().Equal("DD 46 d");
        documentation.Opcodes[0].Instruction.Should().Equal("LD B, (IX + d)");
        documentation.Opcodes[0].TStates.Should().SequenceEqual(19);
        documentation.References.Select(r => r.Covers).Should().SequenceEqual(ReferenceCoverage.Documented, ReferenceCoverage.All);
    }

    [Test]
    public void Create_PrefixedDuplicatesAreUndocumented()
    {
        var documentation = GetDocumentation(Z80Documentation, "NOP");

        documentation.Undocumented.Should().BeFalse();
        documentation.Opcodes.Single(o => o.Opcode == "00").Undocumented.Should().BeFalse();
        documentation.Opcodes.Single(o => o.Opcode == "DD 00").Undocumented.Should().BeTrue();
        documentation.Opcodes.Single(o => o.Opcode == "FD 00").Undocumented.Should().BeTrue();
        documentation.Opcodes.Single(o => o.Opcode == "ED 00").Undocumented.Should().BeTrue();
    }

    [Test]
    public void Create_DuplicateOpcodesAreUndocumented()
    {
        var documentation = GetDocumentation(Z80Documentation, "NEG");

        documentation.Opcodes.Select(o => o.Opcode).Should().SequenceEqual("ED 44", "ED 4C", "ED 54", "ED 5C", "ED 64", "ED 6C", "ED 74", "ED 7C");
        documentation.Opcodes.Select(o => o.Undocumented).Should().SequenceEqual(false, true, true, true, true, true, true, true);
    }

    [Test]
    public void Create_UndocumentedRegistersMakeOpcodesUndocumented()
    {
        var documentation = GetDocumentation(Z80Documentation, "LD R0, n");

        documentation.Opcodes.Single(o => o.Opcode == "06 n").Undocumented.Should().BeFalse();
        documentation.Opcodes.Single(o => o.Opcode == "DD 06 n").Undocumented.Should().BeTrue();
        documentation.Opcodes.Single(o => o.Instruction == "LD IXH, n").Undocumented.Should().BeTrue();
        documentation.Opcodes.Single(o => o.Instruction == "LD IYL, n").Undocumented.Should().BeTrue();
    }

    [Test]
    public void Create_UndocumentedInstruction()
    {
        var documentation = GetDocumentation(Z80Documentation, "SLL R0");

        documentation.Undocumented.Should().BeTrue();
        documentation.Opcodes.Should().OnlyContain(o => o.Undocumented);
        documentation.References.Select(r => r.Covers).Should().SequenceEqual(ReferenceCoverage.All, ReferenceCoverage.Undocumented);
    }

    [Test]
    public void Create_ExplicitlyUndocumentedOpcodes()
    {
        // The documented BIT b, (IX + d) opcodes are the ones with 110 in the low three bits; the rest are marked undocumented.
        var documentation = GetDocumentation(Z80Documentation, "BIT N0, (IX + d)");

        documentation.Undocumented.Should().BeFalse();
        documentation.Opcodes.Single(o => o.Opcode == "DD CB d 46").Undocumented.Should().BeFalse();
        documentation.Opcodes.Single(o => o.Opcode == "DD CB d 40").Undocumented.Should().BeTrue();
        documentation.Opcodes.Single(o => o.Opcode == "DD CB d 47").Undocumented.Should().BeTrue();
        documentation.Opcodes.Single(o => o.Opcode == "DD CB d 7E").Undocumented.Should().BeFalse();
    }

    [Test]
    public void Create_MergesEntriesWithTheSameInstruction()
    {
        var documentation = GetDocumentation(Z80Documentation, "CCF");

        documentation.Opcodes.Select(o => o.Opcode).Should().SequenceEqual("3F", "DD 3F", "FD 3F");
        documentation.Opcodes.Select(o => o.Undocumented).Should().SequenceEqual(false, true, true);
        documentation.Flags.Single(f => f.Flag.Name == "C").Documentation.Should().Equal("Inverted.");
    }

    [Test]
    public void Create_FlagDocumentationOverridesTheDescription()
    {
        var documentation = GetDocumentation(Z80Documentation, "DAA");

        documentation.FlagFallbacks.Should().BeEmpty();
        documentation.Flags.Single(f => f.Flag.Name == "C").Kind.Should().Equal(FlagEffectKind.Changed);
        documentation.Flags.Single(f => f.Flag.Name == "C").Documentation.Should().NotContain("`");
    }

    [Test]
    public void Create_ConditionalInstruction()
    {
        var documentation = GetDocumentation(Z80Documentation, "JR C0, d");

        documentation.Symbolic.Should().Equal("JR cc, d");
        documentation.Opcodes.Where(o => !o.Undocumented).Select(o => o.Instruction).Should().SequenceEqual("JR NZ, d", "JR Z, d", "JR NC, d", "JR C, d");
        documentation.Opcodes.Where(o => !o.Undocumented).Should().OnlyContain(o => o.TStates.SequenceEqual(new[] { 7, 12 }));
        documentation.Opcodes.Single(o => o.Opcode == "DD 20 d").TStates.Should().SequenceEqual(11, 16);
    }

    [Test]
    public void Create_FlagDocumentationUsesPlaceholders()
    {
        var documentation = GetDocumentation(Z80Documentation, "ADD A, R0");

        documentation.Flags.Single(f => f.Flag.Name == "Z").Documentation.Should().Equal("Set if A is zero, otherwise reset.");
        documentation.Documentation.Should().Contain("{R0}");

        var sll = GetDocumentation(Z80Documentation, "SLL R0");
        sll.Flags.Single(f => f.Flag.Name == "C").Documentation.Should().Equal("Set if bit 7 of the original value of {R0} is set, otherwise reset.");
    }

    [Test]
    public void Create_Z80N()
    {
        Z80NDocumentation.Count.Should().BeGreaterThan(Z80Documentation.Count);

        var inherited = GetDocumentation(Z80NDocumentation, "LD R0, (RP0 + d)");
        inherited.Cpu.Should().Equal("Z80N");
        inherited.DefinedBy.Should().Equal("Z80");
        inherited.References.Select(r => r.Covers).Should().SequenceEqual(ReferenceCoverage.Documented, ReferenceCoverage.All);

        var own = GetDocumentation(Z80NDocumentation, "LDIRX");
        own.Cpu.Should().Equal("Z80N");
        own.DefinedBy.Should().Equal("Z80N");
        own.Group.Should().Equal("Z80N Block Transfer");
        own.Page.Should().Equal("z80n-block-transfer/ldirx.md");
        own.Opcodes.Single().TStates.Should().SequenceEqual(16, 21);
        own.References.Should().HaveCount(1);
    }

    [Test]
    public void Create_Z80NOverridesLoseTheirZ80Opcodes()
    {
        var nop = GetDocumentation(Z80NDocumentation, "NOP");

        nop.Opcodes.Any(o => o.Opcode == "ED 23").Should().BeFalse();
        GetDocumentation(Z80Documentation, "NOP").Opcodes.Any(o => o.Opcode == "ED 23").Should().BeTrue();
    }

    [TestCase("LD R0, (RP0 + d)", "ld-r0-at-rp0-d")]
    [TestCase("LD RP0, (nn)", "ld-rp0-at-nn")]
    [TestCase("EX AF, AF'", "ex-af-af-shadow")]
    [TestCase("NEXTREG $n, $n", "nextreg-n-n")]
    [TestCase("8-Bit Load", "8-bit-load")]
    [TestCase("Bit Set, Reset, and Test", "bit-set-reset-and-test")]
    public void Slug(string text, string expected) => CpuDocumentation.Slug(text).Should().Equal(expected);
}