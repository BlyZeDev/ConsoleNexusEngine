namespace ConsoleNexusEngine.Internal;

using System.Text;

internal abstract class BaseCpuMonitor : ICpuMonitor
{
    protected readonly Func<bool> _initializeOls;
    protected readonly Action _deinitializeOls;

    protected readonly Rdmsr _rdmsr;
    protected readonly Cpuid _cpuid;

    public BaseCpuMonitor()
    {
        _initializeOls = Native.Is64BitOS ? Native.InitializeOls_64 : Native.InitializeOls_32;
        _deinitializeOls = Native.Is64BitOS ? Native.DeinitializeOls_64 : Native.DeinitializeOls_32;

        _rdmsr = Native.Is64BitOS ? Native.Rdmsr_64 : Native.Rdmsr_32;
        _cpuid = Native.Is64BitOS ? Native.Cpuid_64 : Native.Cpuid_32;
    }

    public string GetModel()
    {
        _initializeOls();

        var cpuName = new SpanBuilder<char>();

        uint eax, ebx, ecx, edx;
        for (var i = 0u; i < 3; i++)
        {
            if (_cpuid(0x80000002 + i, out eax, out ebx, out ecx, out edx))
            {
                cpuName.Append(ConvertToString(eax));
                cpuName.Append(ConvertToString(ebx));
                cpuName.Append(ConvertToString(ecx));
                cpuName.Append(ConvertToString(edx));
            }
        }

        _deinitializeOls();

        return new string(cpuName.AsReadOnlySpan());
    }

    public abstract int GetTemperature();

    public void Shutdown() => Service.Cleanup();

    private static ReadOnlySpan<char> ConvertToString(uint value)
    {
        var bytes = BitConverter.GetBytes(value).AsSpan();
        return Encoding.ASCII.GetString(bytes);
    }

    protected delegate bool Rdmsr(uint index, out uint eax, out uint edx);

    protected delegate bool Cpuid(uint index, out uint eax, out uint ebx, out uint ecx, out uint edx);
}