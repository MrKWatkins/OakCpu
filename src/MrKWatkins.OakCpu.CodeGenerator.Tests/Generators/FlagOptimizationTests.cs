using MrKWatkins.OakCpu.CodeGenerator.Generators;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Actions;
using MrKWatkins.OakCpu.CodeGenerator.Generators.Flags.Optimization;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Generators;

public sealed class FlagOptimizationTests : TestFixture
{
    [Test]
    public void PerformAllOptimizations_WithSingleConstant_ReturnsTheOriginalConstant()
    {
        var context = new StatementGeneratorContext(Z80GeneratorContext, null);
        var flag = context.Configuration.Flags.Values.OrderBy(f => f.Index).First();
        var constant = new Constant([flag], flag.BitMask);

        var result = FlagOptimization.PerformAllOptimizations(context, [constant], []);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(ReferenceEquals(result[0], constant), Is.True);
    }

    [Test]
    public void PerformAllOptimizations_WithMultipleConstants_CombinesThem()
    {
        var context = new StatementGeneratorContext(Z80GeneratorContext, null);
        var flags = context.Configuration.Flags.Values.OrderBy(f => f.Index).Take(2).ToArray();
        var firstFlag = flags[0];
        var secondFlag = flags[1];
        var actions = new FlagAction[]
        {
            new Constant([firstFlag], firstFlag.BitMask),
            new Constant([secondFlag], 0)
        };

        var result = FlagOptimization.PerformAllOptimizations(context, actions, []);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.TypeOf<Constant>());
        var combined = (Constant)result[0];
        Assert.That(combined.Flags, Is.EqualTo([firstFlag, secondFlag]));
        Assert.That(combined.BitMask, Is.EqualTo(firstFlag.BitMask));
    }
}