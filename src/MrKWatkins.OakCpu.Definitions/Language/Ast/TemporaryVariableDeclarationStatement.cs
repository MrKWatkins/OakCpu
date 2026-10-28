
using System.Text;

namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public sealed class TemporaryVariableDeclarationStatement(TemporaryVariable variable) : Statement
{
    public TemporaryVariable Variable { get; } = variable;

    public override void WriteStringRepresentation(StringBuilder stringRepresentation) => stringRepresentation.Append(Variable);
}