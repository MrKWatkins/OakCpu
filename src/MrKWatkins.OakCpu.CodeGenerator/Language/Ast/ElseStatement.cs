using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class ElseStatement : Statement
{
    public static readonly ElseStatement Instance = new();

    private ElseStatement()
    {
    }

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => throw new NotSupportedException("else statements should be transitory.");
}