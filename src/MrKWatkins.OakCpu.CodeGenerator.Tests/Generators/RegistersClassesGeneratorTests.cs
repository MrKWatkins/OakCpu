using MrKWatkins.OakCpu.CodeGenerator.Generators;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class RegistersClassesGeneratorTests : TestFixture
{
    [Test]
    public void Generate() => RegistersClassesGenerator.Instance.Invoking(g => g.GenerateCompilationUnit(Z80GeneratorContext)).Should().NotThrow();

    [Test]
    public void GenerateOutput()
    {
        string result;
        try
        {
            result = RegistersClassesGenerator.Instance.Generate(Z80GeneratorContext);
        }
        catch (Exception ex)
        {
            throw new Exception($"RegistersClassesGenerator.Generate failed: {ex.Message}", ex);
        }

        // First ensure we have valid output
        result.Should().NotBeNull();
        (result.Length > 0).Should().BeTrue();

        // Validate the structure of the generated registers classes
        result.StartsWith("using System.Runtime.CompilerServices;", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("namespace MrKWatkins.OakCpu.Z80", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public sealed class Z80Registers", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public sealed class Z80ShadowRegisters", StringComparison.Ordinal).Should().BeTrue();

        // Validate main registers class structure
        result.Contains("internal Z80Registers(Z80Emulator emulator)", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("this.emulator = emulator;", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public Z80ShadowRegisters Shadow { get; }", StringComparison.Ordinal).Should().BeTrue();

        // Validate key Z80 registers are present
        result.Contains("public byte A", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public ushort AF", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public byte B", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public ushort BC", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public ushort PC", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public ushort SP", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public ushort IX", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("public ushort IY", StringComparison.Ordinal).Should().BeTrue();

        // Validate shadow registers class structure
        result.Contains("internal Z80ShadowRegisters(Z80Emulator emulator)", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("emulator.Shadow_AF", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("emulator.Shadow_BC", StringComparison.Ordinal).Should().BeTrue();

        // Validate the code uses proper patterns
        result.Contains("[MethodImpl(MethodImplOptions.AggressiveInlining)]", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("get =>", StringComparison.Ordinal).Should().BeTrue();
        result.Contains("set =>", StringComparison.Ordinal).Should().BeTrue();

        // Ensure it's valid C# syntax by checking proper closing
        result.EndsWith("}", StringComparison.Ordinal).Should().BeTrue();

        // Validate reasonable length (should be substantial but not empty)
        (result.Length > 5000).Should().BeTrue();
    }
}