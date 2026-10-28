using System.Text;

namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public sealed class EndIfStatement : Statement
{
    public static readonly EndIfStatement Instance = new();

    private EndIfStatement()
    {
    }

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => throw new NotSupportedException("endif statements should be transitory.");
}