namespace ConsoleNexusEngine.Helpers;

using System.Diagnostics;
using System.IO;

/// <summary>
/// Useful helper methods for <see cref="ConsoleNexusEngine"/>
/// </summary>
public static class NexusEngineHelper
{
    /// <summary>
    /// <see langword="true"/> if the console window is a conhost.exe process, otherwise <see langword="false"/>
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public static bool IsConsoleHost() => Native.GetWindowLong(Native.GetConsoleWindow(), -16) > 0;

    /// <summary>
    /// Starts the program in a new conhost.exe window
    /// </summary>
    public static void StartAppInConhost()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = Path.Combine(Environment.SystemDirectory, "conhost.exe"),
            Arguments = Environment.ProcessPath ?? throw new FileNotFoundException("The .exe path of the process couldn't be found"),
            UseShellExecute = false
        });
    }

    /// <summary>
    /// <see langword="true"/> if the button is pressed, otherwise <see langword="false"/>
    /// </summary>
    /// <remarks>
    /// Basically a better <see cref="Enum.HasFlag(Enum)"/> for <see cref="NexusXInput"/> in every way
    /// </remarks>
    /// <param name="buttons">The pressed buttons</param>
    /// <param name="button">The button that should be checked</param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsPressed(this NexusXInput buttons, NexusXInput button) => (buttons & button) != 0;
}