namespace ConsoleNexusEngine.Internal;

internal sealed class EmptyMonitor : IMonitor
{
    private const int NoTemperature = -1;

    public EmptyMonitor() { }

    public string GetModel() => "Not Found";

    public int GetTemperature() => NoTemperature;

    public void Shutdown() { }
}