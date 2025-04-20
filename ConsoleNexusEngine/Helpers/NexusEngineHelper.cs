namespace ConsoleNexusEngine.Helpers;

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;

/// <summary>
/// Useful helper methods for <see cref="ConsoleNexusEngine"/>
/// </summary>
public static class NexusEngineHelper
{
    /// <summary>
    /// <see langword="true"/> if the console window is a supported console, otherwise <see langword="false"/>
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public static bool IsSupportedConsole() => Native.GetWindowLong(Native.GetConsoleWindow(), -16) > 0;

    /// <summary>
    /// <see langword="true"/> if the program has administrator privileges, otherwise <see langword="false"/>
    /// </summary>
    /// <returns></returns>
    public static bool IsRunAsAdmin()
    {
        var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    /// <summary>
    /// Starts the program in a supported console window.<br/>
    /// Only returns <see langword="false"/> if <paramref name="runAsAdmin"/> was set to <see langword="true"/> and the user declined to start as admin. Otherwise returns <see langword="true"/>
    /// </summary>
    /// <param name="runAsAdmin"><see langword="true"/> if the program should be run as administrator, otherwise <see langword="false"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool StartInSupportedConsole(bool runAsAdmin = false)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.SystemDirectory, "conhost.exe"),
                Arguments = Environment.ProcessPath ?? throw new FileNotFoundException("The .exe path of the process couldn't be found"),
                UseShellExecute = true,
                Verb = runAsAdmin ? "runas" : ""
            });

            return true;
        }
        catch (Win32Exception)
        {
            return false;
        }
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