namespace ConsoleNexusEngine.Internal.WIN;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct PROCESS_BASIC_INFORMATION
{
    public int ExitStatus;
    public int PebBaseAddress;
    public int AffinityMask;
    public int BasePriority;
    public int UniqueProcessId;
    public int InheritedFromUniqueProcessId;
}