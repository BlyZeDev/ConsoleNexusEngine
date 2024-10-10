namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static partial class Native
{
    [DllImport(Advapi, SetLastError = true)]
    public static extern nint CreateService(
        nint hSCManager,
        string lpServiceName,
        string lpDisplayName,
        uint dwDesiredAccess,
        uint dwServiceType,
        uint dwStartType,
        uint dwErrorControl,
        string lpBinaryPathName,
        string lpLoadOrderGroup,
        nint lpdwTagId,
        string lpDependencies,
        string lpServiceStartName,
        string lpPassword);

    [DllImport(Advapi, SetLastError = true)]
    public static extern nint OpenSCManager(string lpMachineName, string lpDatabaseName, uint dwDesiredAccess);

    [DllImport(Advapi, SetLastError = true)]
    public static extern bool StartService(nint hService, int dwNumServiceArgs, string lpServiceArgVectors);

    [DllImport(Advapi, SetLastError = true)]
    public static extern nint OpenService(nint hSCManager, string lpServiceName, uint dwDesiredAccess);

    [DllImport(Advapi, SetLastError = true)]
    public static extern bool DeleteService(nint hService);

    [DllImport(Advapi, SetLastError = true)]
    public static extern bool CloseServiceHandle(nint hSCObject);

    [DllImport(Kernel32, SetLastError = true)]
    public static extern nint CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        nint lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        nint hTemplateFile);    
}