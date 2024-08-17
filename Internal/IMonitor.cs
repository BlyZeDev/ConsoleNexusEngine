namespace ConsoleNexusEngine.Internal;

internal interface IMonitor
{
    public string GetModel();

    public int GetTemperature();

    public void Shutdown();
}