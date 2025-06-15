using System.Text;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

// Not using MrKWatkins.Ast as it does not (currently) support .NET Standard 2.0.
public abstract class AstNode
{
    private string? toString;

    public override string ToString()
    {
        if (toString == null)
        {
            var stringBuilder = new StringBuilder();
            WriteStringRepresentation(stringBuilder);
            toString = stringBuilder.ToString();
        }

        return toString;
    }

    public abstract void WriteStringRepresentation(StringBuilder stringRepresentation);
}