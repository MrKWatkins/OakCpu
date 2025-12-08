using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class CallStatement : Statement
{
    internal CallStatement(Call call)
    {
        Call = call;
    }

    public Call Call { get; }

    public bool IsTerminal => Call.Function.IsTerminal;

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => Call.WriteStringRepresentation(stringRepresentation);

    public override IEnumerable<AstNode> Children
    {
        get
        {
            yield return Call;
        }
    }
}