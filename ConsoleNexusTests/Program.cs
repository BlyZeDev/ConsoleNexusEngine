namespace ConsoleNexusTests;

using ConsoleNexusEngine;
using ConsoleNexusEngine.Graphics;
using ConsoleNexusEngine.Helpers;
using Figgle.Fonts;

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
    private NexusSimpleSprite sprite;

    public Game()
    {
        Settings.Font = new NexusFont("Consolas", new NexusSize(20));
        Settings.ColorPalette = new WindowsColorPalette();
    }

    protected override void Load()
    {
        sprite = new NexusCompoundSpriteBuilder(new NexusText("Width: " + BufferSize.Width, NexusColorIndex.Color3), 0)
            .AddSprite(new NexusCoord(0, 1), new NexusText("Height: " + BufferSize.Height, NexusColorIndex.Color3))
            .AddSprite(new NexusCoord(5, 2), new NexusText("Lol Test", NexusColorIndex.Color3))
            .AddSprite(new NexusCoord(25, 20), new NexusText("Lol Test", NexusColorIndex.Color3))
            .AddSprite(new NexusCoord(25, 1), new NexusText("Height: " + BufferSize.Height, NexusColorIndex.Color3))
            .AddSprite(new NexusEllipse(new NexusSize(68, 66), new NexusChar('-', NexusColorIndex.Color9), true))
            .AddSprite(new NexusCoord(0, 0), new NexusText("-\0+----------", NexusColorIndex.Color4))
            .AddPixel(new NexusCoord(21, 11), new NexusChar('8', NexusColorIndex.Color9))
            .AddPixel(new NexusCoord(71, 71), new NexusChar('7', NexusColorIndex.Color4))
            .AddPixels(new NexusChar('9', NexusColorIndex.Color4), -1, new NexusCoord(0, 0), new NexusCoord(71, 71), new NexusCoord(30, 30), new NexusCoord(31, 31), new NexusCoord(32, 32), new NexusCoord(33, 34))
            .AddLine(new NexusCoord(BufferSize.Width - 1, 0), new NexusCoord(0, BufferSize.Height - 1), new NexusChar('+', NexusColorIndex.Color7))
            .AddSprite(new NexusFiggleText("Test", FiggleFonts.Banner3D, NexusColorIndex.Color7))
            .Build();

        //var first = NexusSpriteExporter.Export(@"C:\Users\leons\Downloads", "Test", sprite, false);

        //sprite = new NexusSimpleSprite(NexusSpriteImporter.Import(first));
    }

    protected override void Update()
    {
        Input.Update();
        Input.UpdateGamepads();

        Graphic.Clear();

        //DebugView();
        Graphic.DrawSprite(NexusCoord.MinValue, sprite);
        Graphic.DrawText(NexusCoord.MinValue, new NexusFiggleText("Settings", FiggleFonts.Banner, NexusColorIndex.Color14, NexusColorIndex.Background));

        Graphic.Render(); throw new Exception();
    }

    protected override void OnCrash(Exception exception)
    {
        Utility.ShowAlert(exception.Message, exception.StackTrace ?? "", NexusAlertIcon.Error, NexusAlertType.Ok, NexusAlertFlags.TopMost);
    }

    protected override void CleanUp()
    {

    }

    private void DebugView()
    {
        var gamepad = Input.Gamepad1;

        Graphic.DrawSprite(NexusCoord.MinValue, new NexusRectangle(BufferSize, new NexusChar(' ', NexusColorIndex.Color11, NexusColorIndex.Color11), true));
        
        Graphic.DrawText(new NexusCoord(0, 0), new NexusText("Type: " + gamepad.Type, NexusColorIndex.Color1));
        Graphic.DrawText(new NexusCoord(1, 1), new NexusText("Battery Type: " + gamepad.BatteryType, NexusColorIndex.Color2));
        Graphic.DrawText(new NexusCoord(2, 2), new NexusText("Battery Level: " + gamepad.BatteryLevel, NexusColorIndex.Color8));
        Graphic.DrawText(new NexusCoord(3, 3), new NexusText("Buttons: " + gamepad.Buttons, NexusColorIndex.Color4));
        Graphic.DrawText(new NexusCoord(4, 4), new NexusText("Left Trigger: " + gamepad.IsLeftTriggerPressed, NexusColorIndex.Color5));
        Graphic.DrawText(new NexusCoord(5, 5), new NexusText("Right Trigger: " + gamepad.IsRightTriggerPressed, NexusColorIndex.Color6));
        Graphic.DrawText(new NexusCoord(6, 6), new NexusText("Left Thumb X: " + gamepad.LeftThumbX, NexusColorIndex.Color7));
        Graphic.DrawText(new NexusCoord(7, 7), new NexusText("Left Thumb Y: " + gamepad.LeftThumbY, NexusColorIndex.Color8));
        Graphic.DrawText(new NexusCoord(8, 8), new NexusText("Right Thumb X: " + gamepad.RightThumbX, NexusColorIndex.Color9));
        Graphic.DrawText(new NexusCoord(9, 9), new NexusText("Right Thumb Y: " + gamepad.RightThumbY, NexusColorIndex.Color10));

        //Graphic.ClearSprite(NexusCoord.MinValue, new NexusRectangle(BufferSize, new NexusChar(' ', NexusColorIndex.Color11, NexusColorIndex.Color11), true));

        Graphic.DrawText(new NexusCoord(10, 10), new NexusText("FPS: " + FramesPerSecond, NexusColorIndex.Color3));
        Graphic.DrawText(new NexusCoord(11, 11), new NexusText("Width: " + BufferSize.Width, NexusColorIndex.Color3));
        Graphic.DrawText(new NexusCoord(12, 12), new NexusText("Height: " + BufferSize.Height, NexusColorIndex.Color3));

        Graphic.DrawLine(new NexusCoord(BufferSize.Width - 5, 0), new NexusCoord(BufferSize.Width - 1, 4), new NexusChar(' ', NexusColorIndex.Color14, NexusColorIndex.Color14));
        Graphic.DrawLine(new NexusCoord(BufferSize.Width - 1, 0), new NexusCoord(BufferSize.Width - 5, 4), new NexusChar(' ', NexusColorIndex.Color14, NexusColorIndex.Color14));

        Graphic.DrawPixel(new NexusCoord(BufferSize.Width - 1, BufferSize.Height - 1), new NexusChar(' ', NexusColorIndex.Color14, NexusColorIndex.Color14));

        Graphic.DrawPixel(new NexusCoord(BufferSize.Width / 2, BufferSize.Height / 2), new NexusChar('X', NexusColorIndex.Color11));

        Graphic.DrawSprite(NexusCoord.MinValue, new NexusRectangle(new NexusSize(2, 2), new NexusChar(' ', NexusColorIndex.Color14, NexusColorIndex.Color14), true));
        Graphic.ClearSprite(NexusCoord.MinValue, new NexusRectangle(new NexusSize(1, 1), new NexusChar(' ', NexusColorIndex.Color14, NexusColorIndex.Color14), true));

        //Debug.WriteLine(DateTime.Now + " - " + (Input.Keys.IsDefaultOrEmpty ? "NO KEY IS PRESSED" : string.Join(',', Input.Keys)) + " - " + Input.MousePosition);
    }
}