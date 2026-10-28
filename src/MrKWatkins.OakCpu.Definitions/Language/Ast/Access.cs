using System.Text;

namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public abstract class Access(string name) : Expression
{
    public string Name { get; } = name;

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => stringRepresentation.Append(Name);
}