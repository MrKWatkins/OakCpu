using System.Collections.Frozen;
using Microsoft.CodeAnalysis.CSharp;

namespace MrKWatkins.OakCpu.CodeGenerator.Language.Ast;

public sealed class Operator
{
    // Precedence taken from https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/#operator-precedence.
    public static readonly Operator Not = new("!", SyntaxKind.LogicalNotExpression, 10, DataType.I32);
    public static readonly Operator BitwiseNot = new("~", SyntaxKind.BitwiseNotExpression, 10, DataType.I32);
    public static readonly Operator Add = new("+", SyntaxKind.AddExpression, 9, DataType.I32, SyntaxKind.AddAssignmentExpression, leftIdentity: 0, rightIdentity: 0);
    public static readonly Operator Subtract = new("-", SyntaxKind.SubtractExpression, 9, DataType.I32, SyntaxKind.SubtractAssignmentExpression, rightIdentity: 0);
    public static readonly Operator LeftShift = new("<<", SyntaxKind.LeftShiftExpression, 8, DataType.Bool, SyntaxKind.LeftShiftAssignmentExpression, rightIdentity: 0);
    public static readonly Operator RightShift = new(">>", SyntaxKind.RightShiftExpression, 8, DataType.Bool, SyntaxKind.RightShiftAssignmentExpression, rightIdentity: 0);
    public static readonly Operator LessThan = new("<", SyntaxKind.LessThanExpression, 7, DataType.Bool);
    public static readonly Operator LessThanOrEqual = new("<=", SyntaxKind.LessThanOrEqualExpression, 7, DataType.Bool);
    public static readonly Operator GreaterThan = new(">", SyntaxKind.GreaterThanExpression, 7, DataType.Bool);
    public static readonly Operator GreaterThanOrEqual = new(">=", SyntaxKind.GreaterThanOrEqualExpression, 7, DataType.Bool);
    public static readonly Operator Equality = new("==", SyntaxKind.EqualsExpression, 6, DataType.Bool);
    public static readonly Operator NotEquals = new("!=", SyntaxKind.NotEqualsExpression, 6, DataType.Bool);
    public static readonly Operator And = new("&", SyntaxKind.BitwiseAndExpression, 5, DataType.I32, SyntaxKind.AndAssignmentExpression);
    public static readonly Operator Xor = new("^", SyntaxKind.ExclusiveOrExpression, 4, DataType.I32, SyntaxKind.ExclusiveOrAssignmentExpression);
    public static readonly Operator Or = new("|", SyntaxKind.BitwiseOrExpression, 3, DataType.I32, SyntaxKind.OrAssignmentExpression, leftIdentity: 0, rightIdentity: 0);
    public static readonly Operator LogicalAnd = new("&&", SyntaxKind.LogicalAndExpression, 2, DataType.Bool);
    public static readonly Operator LogicalOr = new("||", SyntaxKind.LogicalOrExpression, 1, DataType.Bool);
    public static readonly Operator Assignment = new("=", SyntaxKind.SimpleAssignmentExpression, 0, DataType.Void);

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
            LogicalAnd,
            LogicalOr,
            Assignment
        }
        .ToFrozenDictionary(o => o.Symbol);

    private Operator(string symbol, SyntaxKind syntaxKind, int precedence, DataType type, SyntaxKind? compoundAssignmentSyntaxKind = null, int? leftIdentity = null, int? rightIdentity = null)
    {
        Symbol = symbol;
        SyntaxKind = syntaxKind;
        Precedence = precedence;
        Type = type;
        CompoundAssignmentSyntaxKind = compoundAssignmentSyntaxKind;
        LeftIdentity = leftIdentity;
        RightIdentity = rightIdentity;
        BindingPower = (Precedence * 2 + 1, Precedence * 2 + 2);
    }

    public string Symbol { get; }

    public SyntaxKind SyntaxKind { get; }

    public int Precedence { get; }

    public DataType Type { get; }

    public SyntaxKind? CompoundAssignmentSyntaxKind { get; }

    public int? LeftIdentity { get; }

    public int? RightIdentity { get; }

    public (int Left, int Right) BindingPower { get; }

    public override string ToString() => Symbol;
}