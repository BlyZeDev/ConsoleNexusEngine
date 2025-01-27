namespace ConsoleNexusTests;

using ConsoleNexusEngine;
using ConsoleNexusEngine.Graphics;
using ConsoleNexusEngine.Helpers;
using ConsoleNexusEngine.IO;

sealed class Program
{
    static void Main()
    {
        if (!NexusEngineHelper.IsSupportedConsole())
        {
            NexusEngineHelper.StartInSupportedConsole(true);
            return;
        }

        using (var game = new Game())
        {
            game.Start();

            game.Stop();
        }

        Console.Write("Test");
    }
}

public sealed class TestGame : ConsoleGame
{
    public TestGame()
    {
        // Set the allowed input types
        Settings.InputTypes = NexusInputType.All;

        // Set the color palette of the console
        Settings.ColorPalette = new WindowsColorPalette();

        // Set the font family and size of the console
        Settings.Font = new TerminalFont(new NexusSize(25));

        // Set the key that should immediately stop the game
        Settings.StopGameKey = NexusKey.Escape;

        // Set the title of the console
        Settings.Title = "My first game";

        // If true, enables hardware monitoring
        Settings.EnableMonitoring = true;
    }

    protected override void Load()
    {

    }

    protected override void Update(in NexusInputCollection inputs)
    {

    }

    protected override void OnCrash(Exception exception)
    {

    }

    protected override void CleanUp()
    {

    }
}

public sealed class Game : ConsoleGame
{
    private readonly NexusAnimation _animation;

    private double timeSince;

    public Game()
    {
        Settings.Font = new TerminalFont(new NexusSize(25));
        Settings.ColorPalette = new WindowsColorPalette();
        Settings.EnableMonitoring = true;

        //_animation = new NexusAnimation(@"C:\Users\leons\Downloads\tenor.gif", new NexusRgbProcessor(Settings.ColorPalette));
    }

    protected override void Load()
    {

    }

    protected override void Update(in NexusInputCollection inputs)
    {
        Graphic.Clear();
        DebugView(inputs);
        Graphic.Render();

        /*
        NexusUpdate.DoEvery(ref timeSince, DeltaTime, TimeSpan.FromSeconds(0.1), () =>
        {
            Graphic.Clear();

            Graphic.DrawText(NexusCoord.MinValue, new NexusText($"FPS: {FramesPerSecond}", NexusColorIndex.Color15, NexusColorIndex.Color1));
            Graphic.DrawAnimation(new NexusCoord(1, 1), _animation);

            Graphic.Render();
        });
        */
    }

    protected override void OnCrash(Exception exception)
    {

    }

    protected override void CleanUp()
    {

    }

    private void DebugView(in NexusInputCollection inputs)
    {
        var gamepad = inputs.Gamepads[0];

        Graphic.ClearRow(0);
        Graphic.ClearRow(1);
        Graphic.ClearRow(2);
        Graphic.ClearRow(3);
        Graphic.ClearRow(4);
        Graphic.ClearRow(5);
        Graphic.ClearRow(6);
        Graphic.ClearRow(7);
        Graphic.ClearRow(8);
        Graphic.ClearRow(9);
        Graphic.ClearRow(10);
        Graphic.ClearRow(11);
        Graphic.ClearRow(12);
        Graphic.ClearRow(13);
        Graphic.ClearRow(14);
        Graphic.ClearRow(15);
        Graphic.ClearRow(16);
        Graphic.ClearRow(17);

        Graphic.DrawText(new NexusCoord(0, 0), new NexusText("Type: " + gamepad.Type, NexusColorIndex.Color1));
        Graphic.DrawText(new NexusCoord(0, 1), new NexusText("Battery Type: " + gamepad.BatteryType, NexusColorIndex.Color2));
        Graphic.DrawText(new NexusCoord(0, 2), new NexusText("Battery Level: " + gamepad.BatteryLevel, NexusColorIndex.Color8));
        Graphic.DrawText(new NexusCoord(0, 3), new NexusText("Buttons: " + gamepad.Buttons, NexusColorIndex.Color4));
        Graphic.DrawText(new NexusCoord(0, 4), new NexusText("Left Trigger: " + gamepad.IsLeftTriggerPressed, NexusColorIndex.Color5));
        Graphic.DrawText(new NexusCoord(0, 5), new NexusText("Right Trigger: " + gamepad.IsRightTriggerPressed, NexusColorIndex.Color6));
        Graphic.DrawText(new NexusCoord(0, 6), new NexusText("Left Thumb X: " + gamepad.LeftThumbX, NexusColorIndex.Color7));
        Graphic.DrawText(new NexusCoord(0, 7), new NexusText("Left Thumb Y: " + gamepad.LeftThumbY, NexusColorIndex.Color8));
        Graphic.DrawText(new NexusCoord(0, 8), new NexusText("Right Thumb X: " + gamepad.RightThumbX, NexusColorIndex.Color9));
        Graphic.DrawText(new NexusCoord(0, 9), new NexusText("Right Thumb Y: " + gamepad.RightThumbY, NexusColorIndex.Color10));

        Graphic.DrawText(new NexusCoord(0, 10), new NexusText("Memory Allocated: " + Monitor.AllocatedMemory + " MB", NexusColorIndex.Color11));
        Graphic.DrawText(new NexusCoord(0, 11), new NexusText("Peak Memory Allocated: " + Monitor.PeakAllocatedMemory + " MB", NexusColorIndex.Color12));

        Graphic.DrawText(new NexusCoord(0, 12), new NexusText("IsAdmin: " + NexusEngineHelper.IsRunAsAdmin(), NexusColorIndex.Color13));

        Graphic.DrawText(new NexusCoord(0, 13), new NexusText("Cpu Name: " + Monitor.CpuModel, NexusColorIndex.Color14));
        Graphic.DrawText(new NexusCoord(0, 14), new NexusText("Cpu Temperature: " + Monitor.CpuTemperature + " °C", NexusColorIndex.Color15));

        Graphic.DrawText(new NexusCoord(0, 15), new NexusText("Gpu Name: " + Monitor.GpuModel, NexusColorIndex.Color1));
        Graphic.DrawText(new NexusCoord(0, 16), new NexusText("Gpu Temperature: " + Monitor.GpuTemperature + " °C", NexusColorIndex.Color2));
        Graphic.DrawText(new NexusCoord(0, 17), new NexusText("FPS: " + FramesPerSecond + " FPS", NexusColorIndex.Color3));
        Graphic.DrawText(new NexusCoord(0, 18), new NexusText("Width: " + BufferSize.Width, NexusColorIndex.Color3));
        Graphic.DrawText(new NexusCoord(0, 19), new NexusText("Height: " + BufferSize.Height, NexusColorIndex.Color3));
    }

    private void DebugViewFull()
    {
        Graphic.DrawShape(NexusCoord.MinValue, new NexusRectangle(BufferSize, new NexusChar(NexusSpecialChar.Cross, NexusColorIndex.Color5), true));
    }
}