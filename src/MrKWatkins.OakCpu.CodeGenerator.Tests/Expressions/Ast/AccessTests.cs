using MrKWatkins.OakCpu.CodeGenerator.Language.Ast;
using Action = MrKWatkins.OakCpu.CodeGenerator.Definitions.Action;

namespace MrKWatkins.OakCpu.CodeGenerator.Tests.Expressions.Ast;

public sealed class AccessTests : TestFixture
{
    [Test]
    public void ArgumentAccess()
    {
        var access = new ArgumentAccess("temp");

        access.Name.Should().Equal("temp");
        access.Type.Should().Equal(DataType.I32);
        access.Identifier.ToString().Should().Equal("temp");
        access.ToString().Should().Equal("temp");
    }

    [Test]
    public void ActionAccess()
    {
        var action = Z80GeneratorContext.Configuration.Actions.Values.First(a => a != Action.None);
        var access = new ActionAccess(action);

        access.Action.Should().Equal(action);
        access.Name.Should().Equal(action.Name);
        access.Type.Should().Equal(DataType.I32);
        access.ToString().Should().Equal(action.Name);
    }

    [Test]
    public void ConditionAccess()
    {
        var condition = Z80GeneratorContext.Configuration.Conditions.Values.First();
        var access = new ConditionAccess(condition);

        access.Condition.Should().Equal(condition);
        access.Type.Should().Equal(DataType.I32Bool);
        access.ToString().Should().Equal(condition.ToString());
    }

    [Test]
    public void DataMemberAccess()
    {
        var dataMember = Z80GeneratorContext.Configuration.UserDefinedDataMembers.Values.First();
        var access = new DataMemberAccess(dataMember);

        access.DataMember.Should().Equal(dataMember);
        access.Identifier.ToString().Should().Equal(dataMember.FieldName);
        access.Type.Should().Equal(dataMember.Type);
    }

    [Test]
    public void OpcodeStepTableAccess()
    {
        var opcodeStepTable = Z80GeneratorContext.Configuration.OpcodeStepTables.NoPrefix;
        var access = new OpcodeStepTableAccess(opcodeStepTable);

        access.OpcodeStepTable.Should().Equal(opcodeStepTable);
        access.Name.Should().Equal(opcodeStepTable.Name);
        access.Type.Should().Equal(DataType.I32);
    }

    [Test]
    public void SequenceAccess()
    {
        var access = new SequenceAccess("opcode_read");

        access.SequenceName.Should().Equal("opcode_read");
        access.Name.Should().Equal("sequence.opcode_read");
        access.Type.Should().Equal(DataType.I32);
        access.ToString().Should().Equal("sequence.opcode_read");
    }

    [Test]
    public void SequenceGroupAccess()
    {
        var access = new SequenceGroupAccess("interrupt_mode");

        access.SequenceGroupName.Should().Equal("interrupt_mode");
        access.Name.Should().Equal("sequence_group.interrupt_mode");
        access.Type.Should().Equal(DataType.I32);
        access.ToString().Should().Equal("sequence_group.interrupt_mode");
    }
}