namespace ConsoleNexusEngine.Internal;

using System;
using System.Diagnostics;

internal sealed class CmdConsole
{
    public Process Process { get; }
    public nint StandardInput { get; }
    public nint StandardOutput { get; }
    public ConsoleBuffer Buffer { get; }

    public nint Handle => Process.Handle;

    public CmdConsole(int height, int width)
    {
        Process = Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            UseShellExecute = true
        }) ?? throw new NullReferenceException("Couldn't create a new Console Window");

        Native.FocusConsoleWindow(Handle);

        StandardInput = Native.GetConsoleStdInput();
        StandardOutput = Native.GetConsoleStdOutput();

        Native.ResizeConsole(Handle, StandardOutput, width, height);

        Buffer = new ConsoleBuffer(width, height);
    }
}