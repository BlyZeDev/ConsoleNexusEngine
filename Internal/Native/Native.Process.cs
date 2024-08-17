namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static partial class Native
{
    [DllImport(Kernel32)]
    public static extern uint GetCurrentProcessId();

    [DllImport(Kernel32)]
    public static extern nint OpenProcess(uint processAccess, bool bInheritHandle, int processId);

    [DllImport(PSAPI)]
    public static extern bool GetProcessMemoryInfo(nint hProcess, out PROCESS_MEMORY_COUNTERS counters, uint size);

    [DllImport(Kernel32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(nint hObject);
}