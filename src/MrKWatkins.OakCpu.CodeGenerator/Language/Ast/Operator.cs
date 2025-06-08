using System.Collections.Frozen;
using Microsoft.CodeAnalysis.CSharp;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class Operator
{
    // Precedence taken from https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/#operator-precedence.
    public static readonly Operator Not = new("!", SyntaxKind.LogicalNotExpression, 8);
    public static readonly Operator BitwiseNot = new("~", SyntaxKind.BitwiseNotExpression, 8);
    public static readonly Operator Add = new("+", SyntaxKind.AddExpression, 7);
    public static readonly Operator Subtract = new("-", SyntaxKind.SubtractExpression, 7);
    public static readonly Operator LeftShift = new("<<", SyntaxKind.LeftShiftExpression, 6);
    public static readonly Operator RightShift = new(">>", SyntaxKind.RightShiftExpression, 6);
    public static readonly Operator LessThan = new("<", SyntaxKind.LessThanExpression, 5);
    public static readonly Operator LessThanOrEqual = new("<=", SyntaxKind.LessThanOrEqualExpression, 5);
    public static readonly Operator GreaterThan = new(">", SyntaxKind.GreaterThanExpression, 5);
    public static readonly Operator GreaterThanOrEqual = new(">=", SyntaxKind.GreaterThanOrEqualExpression, 5);
    public static readonly Operator Equality = new("==", SyntaxKind.EqualsExpression, 4);
    public static readonly Operator NotEquals = new("!=", SyntaxKind.NotEqualsExpression, 4);
    public static readonly Operator And = new("&", SyntaxKind.BitwiseAndExpression, 3);
    public static readonly Operator Xor = new("^", SyntaxKind.ExclusiveOrExpression, 2);
    public static readonly Operator Or = new("|", SyntaxKind.BitwiseOrExpression, 1);
    public static readonly Operator Assignment = new("=", SyntaxKind.SimpleAssignmentExpression, 0);

    public static readonly IReadOnlyDictionary<string, Operator> UnaryOperators = new[] { Not, BitwiseNot }.ToFrozenDictionary(o => o.Symbol);

    public static readonly IReadOnlyDictionary<string, Operator> BinaryOperators = new[]
        {
            Add,
            Subtract,
            LeftShift,
            RightShift,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual,
            Equality,
            NotEquals,
            And,
            Xor,
            Or,
            Assignment
        }
        .ToFrozenDictionary(o => o.Symbol);

    private Operator(string symbol, SyntaxKind syntaxKind, int precedence)
    {
        Symbol = symbol;
        SyntaxKind = syntaxKind;
        Precedence = precedence;
        BindingPower = (Precedence * 2 + 1, Precedence * 2 + 2);
    }

    public string Symbol { get; }

    public SyntaxKind SyntaxKind { get; }

    public int Precedence { get; }

    public (int Left, int Right) BindingPower { get; }

    public override string ToString() => Symbol;
}