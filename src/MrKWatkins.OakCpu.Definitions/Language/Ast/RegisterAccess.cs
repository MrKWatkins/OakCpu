
namespace MrKWatkins.OakCpu.Definitions.Language.Ast;

public sealed class RegisterAccess(Register register) : Access(register.Name)
{
    public Register Register { get; } = register;

    public override DataType Type => Register.Type;
}