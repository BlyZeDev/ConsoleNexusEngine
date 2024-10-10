namespace ConsoleNexusEngine.Internal;

internal interface ICpuMonitor : IMonitor
{
    public static IMonitor GetMonitor()
    {
        var worked = Service.InstallAndStart();
        if (!worked) return new EmptyMonitor();

        if (IntelCpuMonitor.IsIntelCpu()) return new IntelCpuMonitor();
        if (AmdCpuMonitor.IsAmdCpu()) return new AmdCpuMonitor();

        return new EmptyMonitor();
    }
}