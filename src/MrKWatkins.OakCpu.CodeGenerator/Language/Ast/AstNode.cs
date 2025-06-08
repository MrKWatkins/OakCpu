using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

// Not using MrKWatkins.Ast as it does not (currently) support .NET Standard 2.0.
public abstract class AstNode
{
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        WriteStringRepresentation(stringBuilder);
        return stringBuilder.ToString();
    }

    public abstract void WriteStringRepresentation(StringBuilder stringRepresentation);
}