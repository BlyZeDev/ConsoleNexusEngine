namespace ConsoleNexusEngine.Internal;

using System.IO;

internal static class Service
{
    private const string ServiceName = "WinRing0_1_2_0";

    private static readonly string _driverPath = Path.Combine(Environment.CurrentDirectory, Native.Is64BitOS ? "WinRing0x64.sys" : "WinRing0.sys");

    public static bool InstallAndStart()
    {
        var scm = Native.OpenSCManager(null!, null!, 0x0002);

        if (scm == nint.Zero) return false;

        var service = Native.CreateService(
            scm,
            ServiceName,
            ServiceName,
            0xF01FF,
            0x00000001,
            0x00000003,
            0x00000001,
            _driverPath,
            null!,
            nint.Zero,
            null!,
            null!,
            null!);

        ulong error;
        if (service == nint.Zero)
        {
            error = Native.GetLastError();
            if (error == 1073)
            {
                service = Native.OpenService(scm, ServiceName, 0xF01FF);
                if (service == nint.Zero)
                {
                    Native.CloseServiceHandle(scm);
                    return false;
                }
            }
            else
            {
                Native.CloseServiceHandle(scm);
                return false;
            }
        }

        var result = Native.StartService(service, 0, null!);
        if (!result)
        {
            error = Native.GetLastError();
            if (error != 0x420)
            {
                Native.DeleteService(service);
                Native.CloseServiceHandle(service);
                Native.CloseServiceHandle(scm);
                return false;
            }
        }

        Native.CloseServiceHandle(service);
        Native.CloseServiceHandle(scm);

        return true;
    }

    public static void Cleanup()
    {
        var scm = Native.OpenSCManager(null!, null!, 0x0002);

        if (scm == nint.Zero)
        {
            return;
        }

        var service = Native.OpenService(scm, ServiceName, 0x00010000);
        if (service != nint.Zero)
        {
            Native.DeleteService(service);
            Native.CloseServiceHandle(service);
        }

        Native.CloseServiceHandle(scm);
    }
}