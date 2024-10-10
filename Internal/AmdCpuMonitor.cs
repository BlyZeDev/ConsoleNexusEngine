namespace ConsoleNexusEngine.Internal;

internal sealed class AmdCpuMonitor : BaseCpuMonitor
{
    private const uint TemperatureIndex = 0xC001029A;

    public override int GetTemperature()
    {
        _initializeOls();
        _rdmsr(TemperatureIndex, out var eax, out var edx);
        _deinitializeOls();

        return (int)((((ulong)edx << 32) | eax) & 0xFFFF) / 1000;
    }

    public static bool IsAmdCpu()
    {
        _ = Native.Is64BitOS ? Native.InitializeOls_64() : Native.InitializeOls_32();

        var result = Native.Is64BitOS
            ? Native.Rdmsr_64(TemperatureIndex, out _, out _)
            : Native.Rdmsr_32(TemperatureIndex, out _, out _);

        if (Native.Is64BitOS) Native.DeinitializeOls_64();
        else Native.DeinitializeOls_32();

        return result;
    }
}