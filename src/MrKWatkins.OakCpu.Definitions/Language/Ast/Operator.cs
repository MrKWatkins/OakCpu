using System.Collections.Frozen;

namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public sealed class Operator
{
    // Precedence taken from https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/#operator-precedence.
    public static readonly Operator Not = new("!", 11, DataType.I32);
    public static readonly Operator BitwiseNot = new("~", 11, DataType.I32);
    public static readonly Operator Multiply = new("*", 10, DataType.I32, compoundAssignment: true, leftIdentity: 1, rightIdentity: 1);
    public static readonly Operator Add = new("+", 9, DataType.I32, compoundAssignment: true, leftIdentity: 0, rightIdentity: 0);
    public static readonly Operator Subtract = new("-", 9, DataType.I32, compoundAssignment: true, rightIdentity: 0);
    public static readonly Operator LeftShift = new("<<", 8, DataType.Bool, compoundAssignment: true, rightIdentity: 0);
    public static readonly Operator RightShift = new(">>", 8, DataType.Bool, compoundAssignment: true, rightIdentity: 0);
    public static readonly Operator LessThan = new("<", 7, DataType.Bool);
    public static readonly Operator LessThanOrEqual = new("<=", 7, DataType.Bool);
    public static readonly Operator GreaterThan = new(">", 7, DataType.Bool);
    public static readonly Operator GreaterThanOrEqual = new(">=", 7, DataType.Bool);
    public static readonly Operator Equality = new("==", 6, DataType.Bool);
    public static readonly Operator NotEquals = new("!=", 6, DataType.Bool);
    public static readonly Operator And = new("&", 5, DataType.I32, compoundAssignment: true);
    public static readonly Operator Xor = new("^", 4, DataType.I32, compoundAssignment: true);
    public static readonly Operator Or = new("|", 3, DataType.I32, compoundAssignment: true, leftIdentity: 0, rightIdentity: 0);
    public static readonly Operator LogicalAnd = new("&&", 2, DataType.Bool);
    public static readonly Operator LogicalOr = new("||", 1, DataType.Bool);
    public static readonly Operator Assignment = new("=", 0, DataType.Void);

    public static readonly IReadOnlyDictionary<string, Operator> UnaryOperators = new[] { Not, BitwiseNot }.ToFrozenDictionary(o => o.Symbol);

    public static readonly IReadOnlyDictionary<string, Operator> BinaryOperators = new[]
        {
            Multiply,
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

    internal static readonly FrozenSet<char> OperatorCharacters = UnaryOperators.Keys
        .Concat(BinaryOperators.Keys)
        .SelectMany(static o => o)
        .ToFrozenSet();

    private Operator(string symbol, int precedence, DataType type, bool compoundAssignment = false, int? leftIdentity = null, int? rightIdentity = null)
    {
        Symbol = symbol;
        Precedence = precedence;
        Type = type;
        HasCompoundAssignment = compoundAssignment;
        LeftIdentity = leftIdentity;
        RightIdentity = rightIdentity;
        BindingPower = (Precedence * 2 + 1, Precedence * 2 + 2);
    }

    public string Symbol { get; }

    public int Precedence { get; }

    public DataType Type { get; }

    /// <summary>
    /// Whether the operator has a compound assignment form, e.g. <c>+=</c> for <c>+</c>.
    /// </summary>
    public bool HasCompoundAssignment { get; }

    public int? LeftIdentity { get; }

    public int? RightIdentity { get; }

    public (int Left, int Right) BindingPower { get; }

    public override string ToString() => Symbol;
}