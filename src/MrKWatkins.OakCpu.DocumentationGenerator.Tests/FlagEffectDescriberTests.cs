namespace MrKWatkins.OakCpu.DocumentationGenerator.Tests;

public sealed class FlagEffectDescriberTests : TestFixture
{
    [Test]
    public void Describe_Unaffected()
    {
        var effects = Describe(Z80Model, "LD B, C", new Dictionary<string, string> { ["B"] = "{R0}", ["C"] = "{R1}" });

        effects.Select(e => e.Flag.Name).Should().SequenceEqual("S", "Z", "Y", "H", "X", "PV", "N", "C");
        effects.Should().OnlyContain(e => e is { Kind: FlagEffectKind.Unaffected, Documentation: "Unaffected." });
    }

    [Test]
    public void Describe_Add()
    {
        var effects = Describe(Z80Model, "ADD A, B", new Dictionary<string, string> { ["B"] = "{R0}" });

        AssertEffect(effects, "S", FlagEffectKind.Changed, "Copied from bit 7 of the result.");
        AssertEffect(effects, "Z", FlagEffectKind.Changed, "Set if A is zero, otherwise reset.");
        AssertEffect(effects, "Y", FlagEffectKind.Changed, "Copied from bit 5 of the result.");
        AssertEffect(effects, "H", FlagEffectKind.Changed, "Set if there was a carry from bit 3 to bit 4, otherwise reset.");
        AssertEffect(effects, "X", FlagEffectKind.Changed, "Copied from bit 3 of the result.");
        AssertEffect(effects, "PV", FlagEffectKind.Changed, "Set if the addition overflowed, otherwise reset.");
        AssertEffect(effects, "N", FlagEffectKind.Reset, "Reset.");
        AssertEffect(effects, "C", FlagEffectKind.Changed, "Set if bit 8 of the result is set, otherwise reset.");
    }

    [Test]
    public void Describe_Placeholders()
    {
        var effects = Describe(Z80Model, "SLL B", new Dictionary<string, string> { ["B"] = "{R0}" });

        AssertEffect(effects, "C", FlagEffectKind.Changed, "Set if bit 7 of the original value of {R0} is set, otherwise reset.");
        AssertEffect(effects, "PV", FlagEffectKind.Changed, "Set if {R0} has even parity, otherwise reset.");
        AssertEffect(effects, "Z", FlagEffectKind.Changed, "Set if {R0} is zero, otherwise reset.");
        AssertEffect(effects, "H", FlagEffectKind.Reset, "Reset.");
    }

    [Test]
    public void Describe_ValueRead()
    {
        var effects = Describe(Z80Model, "IN B, (C)", new Dictionary<string, string> { ["B"] = "{R0}" });

        AssertEffect(effects, "S", FlagEffectKind.Changed, "Copied from bit 7 of the value read.");
        AssertEffect(effects, "PV", FlagEffectKind.Changed, "Set if the value read has even parity, otherwise reset.");
    }

    [Test]
    public void Describe_Result()
    {
        var effects = Describe(Z80Model, "RLC (HL)", new Dictionary<string, string>());

        AssertEffect(effects, "C", FlagEffectKind.Changed, "Set if bit 7 of the value read is set, otherwise reset.");
        AssertEffect(effects, "S", FlagEffectKind.Changed, "Copied from bit 7 of the result.");
    }

    [Test]
    public void Describe_PreviousFlags()
    {
        var effects = Describe(Z80Model, "CCF", new Dictionary<string, string>());

        AssertEffect(effects, "C", FlagEffectKind.Changed, "Inverted.");
        AssertEffect(effects, "H", FlagEffectKind.Changed, "Set to the previous value of the C flag.");
        AssertEffect(effects, "N", FlagEffectKind.Reset, "Reset.");
    }

    [Test]
    public void Describe_InterruptFlipFlop()
    {
        var effects = Describe(Z80Model, "LD A, I", new Dictionary<string, string> { ["I"] = "{R0}" });

        AssertEffect(effects, "PV", FlagEffectKind.Changed, "Set to the value of IFF2.");
        AssertEffect(effects, "Z", FlagEffectKind.Changed, "Set if {R0} is zero, otherwise reset.");
    }

    [Test]
    public void Describe_Comparisons()
    {
        var effects = Describe(Z80Model, "NEG", new Dictionary<string, string>());

        AssertEffect(effects, "C", FlagEffectKind.Changed, "Set if A is not zero, otherwise reset.");
        AssertEffect(effects, "PV", FlagEffectKind.Changed, "Set if A equals 0x80, otherwise reset.");
        AssertEffect(effects, "H", FlagEffectKind.Changed, "Set if bit 4 of (the original value of A ^ A) is set, otherwise reset.");
    }

    [Test]
    public void Describe_InlinesTemporaries()
    {
        var effects = Describe(Z80Model, "CPI", new Dictionary<string, string>());

        AssertEffect(effects, "X", FlagEffectKind.Changed, "Set if bit 3 of (the result - the half borrow) is set, otherwise reset.");
        AssertEffect(effects, "H", FlagEffectKind.Changed, "Set if there was a borrow from bit 4, otherwise reset.");
        AssertEffect(effects, "PV", FlagEffectKind.Changed, "Set if BC is not zero, otherwise reset.");
    }

    [Test]
    public void Describe_BlockOutput()
    {
        var effects = Describe(Z80Model, "OUTI", new Dictionary<string, string>());

        AssertEffect(effects, "C", FlagEffectKind.Changed, "Set if (the value read + (L & 0xFF)) is greater than 0xFF, otherwise reset.");
        AssertEffect(effects, "Z", FlagEffectKind.Changed, "Set if B is zero, otherwise reset.");
    }

    [Test]
    public void Describe_StatusRegisterWrites()
    {
        var effects = Describe(M6502Model, "CLC", new Dictionary<string, string>());
        effects.Select(e => e.Flag.Name).Should().SequenceEqual("N", "V", "B", "D", "I", "Z", "C");
        AssertEffect(effects, "C", FlagEffectKind.Reset, "Reset.");
        AssertEffect(effects, "Z", FlagEffectKind.Unaffected, "Unaffected.");

        effects = Describe(M6502Model, "SEI", new Dictionary<string, string>());
        AssertEffect(effects, "I", FlagEffectKind.Set, "Set.");

        effects = Describe(M6502Model, "ASL A", new Dictionary<string, string>());
        AssertEffect(effects, "C", FlagEffectKind.Changed, "Set if bit 7 of the original value of A is set, otherwise reset.");
        AssertEffect(effects, "Z", FlagEffectKind.Changed, "Set if A is zero, otherwise reset.");
        AssertEffect(effects, "N", FlagEffectKind.Changed, "Set if bit 7 of A is set, otherwise reset.");
        AssertEffect(effects, "V", FlagEffectKind.Unaffected, "Unaffected.");

        effects = Describe(M6502Model, "INC zp", new Dictionary<string, string>());
        AssertEffect(effects, "Z", FlagEffectKind.Changed, "Set if the result is zero, otherwise reset.");
    }

    [Test]
    public void Describe_StatusRegisterLoad()
    {
        var effects = Describe(M6502Model, "PLP", new Dictionary<string, string>());

        AssertEffect(effects, "N", FlagEffectKind.Changed, "Copied from bit 7 of the value read.");
        AssertEffect(effects, "B", FlagEffectKind.Reset, "Reset.");
        AssertEffect(effects, "C", FlagEffectKind.Changed, "Copied from bit 0 of the value read.");
    }

    [Test]
    public void Describe_FollowsSequences()
    {
        // LDA abs,X finishes in a named sequence shared by its paths.
        var effects = Describe(M6502Model, "LDA abs,X", new Dictionary<string, string>());

        AssertEffect(effects, "Z", FlagEffectKind.Changed, "Set if A is zero, otherwise reset.");
        AssertEffect(effects, "N", FlagEffectKind.Changed, "Set if bit 7 of A is set, otherwise reset.");
        AssertEffect(effects, "C", FlagEffectKind.Unaffected, "Unaffected.");
    }

    [Test]
    public void Describe_Modes()
    {
        var effects = Describe(M6502Model, "SBC #n", new Dictionary<string, string>());

        AssertEffect(effects, "Z", FlagEffectKind.Changed, "In decimal mode, set if the low byte of the binary result is zero, otherwise reset. In binary mode, set if A is zero, otherwise reset.");
        AssertEffect(effects, "V", FlagEffectKind.Changed, "Set if the subtraction overflowed, otherwise reset.");
        AssertEffect(effects, "C", FlagEffectKind.Changed, "Set if bit 8 of the binary result is reset, otherwise reset.");
    }

    [Test]
    public void Describe_UserDefinedFunctions()
    {
        var effects = Describe(M6502Model, "CMP zp", new Dictionary<string, string>());

        AssertEffect(effects, "C", FlagEffectKind.Changed, "Set if A is greater than or equal to the value read, otherwise reset.");
        AssertEffect(effects, "N", FlagEffectKind.Changed, "Set if bit 7 of the result is set, otherwise reset.");
    }

    [Test]
    public void Describe_SetsFlagsTogether()
    {
        // ANC sets C with N in one write.
        var effects = Describe(M6502Model, "ANC #n", new Dictionary<string, string>());

        AssertEffect(effects, "N", FlagEffectKind.Changed, "Set if bit 7 of A is set, otherwise reset.");
        AssertEffect(effects, "C", FlagEffectKind.Changed, "Set if bit 7 of A is set, otherwise reset.");
    }

    [Test]
    public void Describe_FallsBackToTheExpression_M6502()
    {
        // ADC's decimal mode adjusts its temporaries in several branches, which cannot be described.
        var instruction = GetInstruction(M6502Model, "ADC #n");
        var describer = new FlagEffectDescriber(M6502Model, instruction, new Dictionary<string, string>());

        var effects = describer.Describe();

        describer.Fallbacks.Select(f => f.Key).Distinct().Order().Should().SequenceEqual("C", "N");
        effects.Single(e => e.Flag.Name == "N").Documentation.Should().Contain("`");
        effects.Single(e => e.Flag.Name == "Z").Documentation.Should().NotContain("`");
    }

    [Test]
    public void Describe_FallsBackToTheExpression()
    {
        // DAA assigns its temporaries in several branches, which cannot be described.
        var instruction = GetInstruction(Z80Model, "DAA");
        var describer = new FlagEffectDescriber(Z80Model, instruction, new Dictionary<string, string>());

        var effects = describer.Describe();

        describer.Fallbacks.Should().NotBeEmpty();
        describer.Fallbacks.Select(f => f.Key).Should().Contain("C");
        effects.Single(e => e.Flag.Name == "C").Documentation.Should().Contain("`");
    }

    [Pure]
    private static IReadOnlyList<FlagEffect> Describe(CpuModel model, string mnemonic, IReadOnlyDictionary<string, string> placeholders)
    {
        var instruction = GetInstruction(model, mnemonic);
        var describer = new FlagEffectDescriber(model, instruction, placeholders);
        var effects = describer.Describe();
        describer.Fallbacks.Should().BeEmpty();
        return effects;
    }

    private static void AssertEffect(IReadOnlyList<FlagEffect> effects, string flag, FlagEffectKind kind, string description)
    {
        var effect = effects.Single(e => e.Flag.Name == flag);
        effect.Kind.Should().Equal(kind);
        effect.Documentation.Should().Equal(description);
    }
}