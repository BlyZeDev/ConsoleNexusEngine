namespace ConsoleNexusTests;

using ConsoleNexusEngine;
using ConsoleNexusEngine.Graphics;
using ConsoleNexusEngine.Helpers;
using ConsoleNexusEngine.IO;
using ConsoleNexusEngine.Sound;

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

public sealed class Game : NexusConsoleGame
{
    private readonly NexusUpdate _everySecond;
    private readonly NexusSound _sound;
    private readonly NexusSound _sfx;

    public Game()
    {
        Settings.Font = new NexusFont("Terminal", new NexusSize(10));
        Settings.ColorPalette = new CGAColorPalette();

        _everySecond = NexusUpdate.DoEvery(TimeSpan.FromMicroseconds(1), (inputs) =>
        {
            Graphic.Clear();
            DebugView(inputs);
            Graphic.Render();
        });
        _sound = new NexusSound(@"C:\Users\leons\Downloads\testaudio.mp3", new NexusVolume(50), true);
        _sfx = new NexusSound(@"C:\Users\leons\Downloads\sfx.wav", new NexusVolume(25), false);
    }

    protected override void Load()
    {
        _sound.Play();
    }

    protected override void Update(NexusInputCollection inputs)
    {
        _everySecond.Update(DeltaTime);

        if (inputs.Keys.Contains(NexusKey.Return))
        {
            if (_sfx.State is NexusPlayerState.NotStarted) _sfx.Play();
            else _sfx.Restart();
        }

        if (inputs.Keys.Contains(NexusKey.Up)) _sound.Volume += 5;
        if (inputs.Keys.Contains(NexusKey.Down)) _sound.Volume -= 5;
    }

    protected override void OnCrash(Exception exception)
    {
        Utility.ShowAlert(exception.Message, exception.StackTrace ?? "", NexusAlertIcon.Error);
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
        Graphic.DrawText(new NexusCoord(0, 10), new NexusText("FPS: " + FramesPerSecond + " FPS", NexusColorIndex.Color3));
        Graphic.DrawText(new NexusCoord(0, 11), new NexusText("Width: " + BufferSize.Width, NexusColorIndex.Color3));
        Graphic.DrawText(new NexusCoord(0, 12), new NexusText("Height: " + BufferSize.Height, NexusColorIndex.Color3));
    }
}