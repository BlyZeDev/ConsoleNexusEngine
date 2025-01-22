namespace ConsoleNexusWPFTests;

using ConsoleNexusEngine.Graphics;
using ConsoleNexusEngine.IO;
using ConsoleNexusEngine;
using System.Windows;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        using (var game = new Game())
        {
            game.Start();

            game.Stop();
        }
    }
}

public sealed class Game : ConsoleGame
{
    private readonly NexusEllipse _ellipse;
    private NexusCoord currentPos;

    public Game()
    {
        Settings.Font = new TerminalFont(new NexusSize(25));
        Settings.Priority = ThreadPriority.Highest;

        _ellipse = new NexusEllipse(new NexusSize(25, 25), new NexusChar(NexusSpecialChar.FullBlock, Settings.ColorPalette.Color15), true);
        currentPos = new NexusCoord(0, 0);
    }

    protected override void Load()
    {
        Controller.AddControl(new NexusInputCondition(NexusKey.A), () => Graphic.SetBackground(Utility.GetRandomColor(true)));
        Controller.AddControl(new NexusInputCondition(NexusKey.W), () => Graphic.SetBackground(Utility.GetRandomColor(true)));
        Controller.AddControl(new NexusInputCondition(new NexusCoord(0, 0)), () => Graphic.SetBackground(Utility.GetRandomColor(true)));
        Controller.AddControl(new NexusInputCondition(NexusKey.S), () => Graphic.SetBackground(Utility.GetRandomColor(true)));
        Controller.AddControl(new NexusInputCondition(NexusKey.D), () => Graphic.SetBackground(Utility.GetRandomColor(true)));
    }

    private double timeSince = 0;
    protected override void Update(in NexusInputCollection inputs)
    {
        Controller.Control(inputs);

        Graphic.DrawText(new NexusCoord(0, 0), new NexusText(FramesPerSecond, Settings.ColorPalette.Color10));
        Graphic.DrawText(new NexusCoord(0, 1), new NexusText(DateTime.Now - StartTime, Settings.ColorPalette.Color11));

        DoEverySecond(ref timeSince, () =>
        {
            Graphic.ClearShape(currentPos, _ellipse);

            currentPos = Utility.GetRandomCoord((BufferSize - new NexusSize(25)).ToCoord());
            Graphic.DrawShape(currentPos, _ellipse);
        });

        Graphic.Render();
    }

    protected override void CleanUp()
    {

    }
}