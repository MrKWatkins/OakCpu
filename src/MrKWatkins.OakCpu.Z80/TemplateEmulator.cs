using System.Runtime.InteropServices;

namespace MrKWatkins.OakCpu.Z80;

[StructLayout(LayoutKind.Explicit)]
public sealed class TemplateEmulator
{
    [FieldOffset(0)]
    internal byte A;
}