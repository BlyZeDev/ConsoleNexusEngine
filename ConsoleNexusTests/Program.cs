namespace ConsoleNexusTests;

using ConsoleNexusEngine;
using ConsoleNexusEngine.Graphics;
using ConsoleNexusEngine.Helpers;

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
        }

        Console.Write("Test");
        Console.Clear();
    }
}

public sealed class Game : NexusConsoleGame
{
    private int counter;

    public Game()
    {
        Settings.Font = new NexusFont("Terminal", new NexusSize(10));
        Settings.ColorPalette = new CGAColorPalette();
    }

    protected override void Load()
    {

    }

    protected override void Update()
    {
        Input.Update();
        Input.UpdateGamepads();

        Graphic.Clear();

        DebugView();

        Graphic.Render();
    }

    protected override void OnCrash(Exception exception)
    {
        Utility.ShowAlert(exception.Message, exception.StackTrace ?? "", NexusAlertIcon.Error);
    }

    protected override void CleanUp()
    {

    }

    private void DebugView()
    {
        var gamepad = Input.Gamepad1;

        //Graphic.DrawText(new NexusCoord(0, 0), new NexusText("Type: " + gamepad.Type, NexusColorIndex.Color1));
        Graphic.DrawText(new NexusCoord(1, 1), new NexusText("Battery Type: " + gamepad.BatteryType, NexusColorIndex.Color2));
        Graphic.DrawText(new NexusCoord(2, 2), new NexusText("Battery Level: " + gamepad.BatteryLevel, NexusColorIndex.Color8));
        Graphic.DrawText(new NexusCoord(3, 3), new NexusText("Buttons: " + gamepad.Buttons, NexusColorIndex.Color4));
        Graphic.DrawText(new NexusCoord(4, 4), new NexusText("Left Trigger: " + gamepad.IsLeftTriggerPressed, NexusColorIndex.Color5));
        Graphic.DrawText(new NexusCoord(5, 5), new NexusText("Right Trigger: " + gamepad.IsRightTriggerPressed, NexusColorIndex.Color6));
        Graphic.DrawText(new NexusCoord(6, 6), new NexusText("Left Thumb X: " + gamepad.LeftThumbX, NexusColorIndex.Color7));
        Graphic.DrawText(new NexusCoord(7, 7), new NexusText("Left Thumb Y: " + gamepad.LeftThumbY, NexusColorIndex.Color8));
        Graphic.DrawText(new NexusCoord(8, 8), new NexusText("Right Thumb X: " + gamepad.RightThumbX, NexusColorIndex.Color9));
        Graphic.DrawText(new NexusCoord(9, 9), new NexusText("Right Thumb Y: " + gamepad.RightThumbY, NexusColorIndex.Color10));
        Graphic.DrawText(new NexusCoord(10, 10), new NexusText("FPS: " + FramesPerSecond, NexusColorIndex.Color3));
        Graphic.DrawText(new NexusCoord(11, 11), new NexusText("Width: " + BufferSize.Width, NexusColorIndex.Color3));
        Graphic.DrawText(new NexusCoord(12, 12), new NexusText("Height: " + BufferSize.Height, NexusColorIndex.Color3));
        var text = new NexusText(new string('X', BufferSize.Width / 2), NexusColorIndex.Color4);
        Graphic.DrawText(new NexusCoord((BufferSize.Width - text.Size.Width) / 2, 13), text);

        Graphic.DrawText(new NexusCoord(0, 0), new NexusText("Type: " + gamepad.Type, NexusColorIndex.Color1));

        //Debug.WriteLine(DateTime.Now + " - " + (Input.Keys.IsDefaultOrEmpty ? "NO KEY IS PRESSED" : string.Join(',', Input.Keys)) + " - " + Input.MousePosition);
    }
}