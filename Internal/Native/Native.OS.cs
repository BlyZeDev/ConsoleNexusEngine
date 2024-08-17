namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static partial class Native
{
    public static string WindowsVersion => RuntimeInformation.OSDescription;

    public static bool Is64BitOS => Environment.Is64BitOperatingSystem;

    [LibraryImport(Kernel32)]
    public static partial uint GetLastError();
}