namespace Snake;

using ConsoleNexusEngine;
using ConsoleNexusEngine.Graphics;
using ConsoleNexusEngine.Helpers;
using ConsoleNexusEngine.IO;
using ConsoleNexusEngine.Sound;
using Figgle;
using System.Runtime.InteropServices;

public sealed class SnakeGame : ConsoleGame
{
    private const int MaxLength = 100;

    private readonly NexusSound _mainTheme;
    private readonly NexusSound _eatSound;
    private readonly NexusSound _gameOver;

    private readonly NexusCoord _playingFieldStart;
    private readonly NexusCoord _playingFieldEnd;
    private readonly NexusCoord _counterPos;

    private readonly NexusRectangle _playingBorder;
    private readonly NexusRectangle _playingField;

    private readonly NexusColorIndex _background;
    private readonly NexusColorIndex _fruit;
    private readonly NexusColorIndex _border;
    private readonly NexusColorIndex _counter;

    private readonly NexusChar _headChar;
    private readonly NexusChar _bodyChar;
    private readonly NexusChar _fruitChar;

    private readonly List<NexusCoord> _snake;

    private bool isGameOver;
    private double snakeMovement;
    private int points;
    private int length;

    private NexusDirection currentDirection;
    private NexusCoord currentFruit;

    public SnakeGame(string directory)
    {
        _mainTheme = new NexusSound(Path.Combine(directory, "MainTheme.mp3"), new NexusVolume(10), true);
        _eatSound = new NexusSound(Path.Combine(directory, "Eat.mp3"), NexusVolume.MaxValue);
        _gameOver = new NexusSound(Path.Combine(directory, "GameOver.mp3"), new NexusVolume(50));

        Settings.ColorPalette = new SnakeColorPalette();
        Settings.EnableMonitoring = false;
        Settings.Font = new TerminalFont(new NexusSize(25));
        Settings.InputTypes = NexusInputType.Keyboard | NexusInputType.Gamepad;
        Settings.Priority = ThreadPriority.Highest;
        Settings.StopGameKey = NexusKey.Escape;
        Settings.Title = "Snake";

        var centerX = BufferSize.Width / 2;
        var centerY = BufferSize.Height / 2;

        var fieldWidth = centerX - BufferSize.Width / 6;
        var fieldHeight = centerY - BufferSize.Height / 6;

        _playingFieldStart = new NexusCoord(centerX - fieldWidth, centerY - fieldHeight);
        _playingFieldEnd = new NexusCoord(centerX + fieldWidth, centerY + fieldHeight);
        _counterPos = _playingFieldStart - new NexusCoord(0, _playingFieldStart.Y / 2);

        _background = NexusColorIndex.Color1;
        _fruit = NexusColorIndex.Color5;
        _border = NexusColorIndex.Color7;
        _counter = NexusColorIndex.Color14;

        _playingField = new NexusRectangle(_playingFieldStart, _playingFieldEnd + new NexusCoord(1, 1), new NexusChar(char.MinValue, NexusColorIndex.Background, _background), true);
        _playingBorder = new NexusRectangle(_playingField.Size + new NexusSize(2), new NexusChar(char.MinValue, NexusColorIndex.Background, _border), false);

        _headChar = new NexusChar(NexusSpecialChar.FullBlock, NexusColorIndex.Color12);
        _bodyChar = new NexusChar(NexusSpecialChar.FullBlock, NexusColorIndex.Color11);
        _fruitChar = new NexusChar(NexusSpecialChar.Diamond, _fruit, _background);

        _snake = [new NexusCoord(_playingFieldEnd.X / 2, _playingFieldEnd.Y / 2)];

        length = 1;

        currentDirection = NexusDirection.Right;
        currentFruit = GetRandomFruit();
    }

    protected override void Load() => _mainTheme.Play();

    protected override void Update(in NexusInputCollection inputs)
    {
        if (isGameOver) return;

        var gamepad = inputs.Gamepads[0].Buttons;
        var keyboard = inputs.Keys;

        if ((gamepad.IsPressed(NexusXInput.DirectionalPadRight)
            || keyboard.Contains(NexusKey.Right))
            && !currentDirection.HasDirection(NexusDirection.Left))
            currentDirection = NexusDirection.Right;

        if ((gamepad.IsPressed(NexusXInput.DirectionalPadLeft)
            || keyboard.Contains(NexusKey.Left))
            && !currentDirection.HasDirection(NexusDirection.Right))
            currentDirection = NexusDirection.Left;

        if ((gamepad.IsPressed(NexusXInput.DirectionalPadUp)
            || keyboard.Contains(NexusKey.Up))
            && !currentDirection.HasDirection(NexusDirection.Down))
            currentDirection = NexusDirection.Up;

        if ((gamepad.IsPressed(NexusXInput.DirectionalPadDown)
            || keyboard.Contains(NexusKey.Down))
            && !currentDirection.HasDirection(NexusDirection.Up))
            currentDirection = NexusDirection.Down;

        NexusUpdate.DoEvery(ref snakeMovement, DeltaTime, TimeSpan.FromMilliseconds(80), () =>
        {
            var newPos = _snake[0];

            var head = _snake[0];
            NexusNavigation.Move(ref head, currentDirection);
            _snake[0] = head;

            for (int i = 1; i < length; i++)
            {
                if (_snake[0] == _snake[i])
                {
                    GameOver();
                    return;
                }

                (_snake[i], newPos) = (newPos, _snake[i]);
            }

            if (!IsInBounds(_snake[0]))
            {
                GameOver();
                return;
            }
        });

        if (isGameOver) return;

        if (_snake[0] == currentFruit)
        {
            _eatSound.Play();

            if (length < MaxLength)
            {
                length++;
                _snake.Add(_snake[0]);
            }

            points++;
            currentFruit = GetRandomFruit();
        }

        Graphic.DrawText(_counterPos - new NexusCoord(0, 1), new NexusText((DateTime.Now - StartTime).ToString(@"hh\:mm\:ss"), _counter));
        Graphic.DrawText(_counterPos, new NexusText($"Points: {points}", _counter));

        Graphic.DrawShape(_playingFieldStart - new NexusCoord(1, 1), _playingBorder);
        Graphic.DrawShape(_playingFieldStart, _playingField);

        Graphic.DrawPixel(currentFruit, _fruitChar);

        Graphic.DrawPixel(_snake[0], _headChar);
        Graphic.DrawPixels(_bodyChar, CollectionsMarshal.AsSpan(_snake[1..]));

        Graphic.Render();
    }

    protected override void CleanUp()
    {
        _mainTheme.Dispose();
        _eatSound.Dispose();
        _gameOver.Dispose();
    }

    private void GameOver()
    {
        isGameOver = true;

        _mainTheme.Stop();
        _gameOver.Play();

        Graphic.Clear();

        var center = new NexusCoord(BufferSize.Width / 2, BufferSize.Height / 2);

        var gameOverText = new NexusFiggleText("Game Over!", FiggleFonts.Banner, _counter);

        if (gameOverText.Size.Width >= BufferSize.Width || gameOverText.Size.Height >= BufferSize.Height)
        {
            var altGameOverText = new NexusText("Game Over!", _counter);

            Graphic.DrawText(center - new NexusCoord(altGameOverText.Size.Width / 2, altGameOverText.Size.Height), altGameOverText);
        }
        else Graphic.DrawText(center - new NexusCoord(gameOverText.Size.Width / 2, gameOverText.Size.Height), gameOverText);

        var pointsText = new NexusText($"You had {points} {(points == 1 ? "point" : "points")}.", _counter);

        Graphic.DrawText(center - new NexusCoord(pointsText.Size.Width / 2, pointsText.Size.Height - 1), pointsText);
        
        Graphic.Render();
    }

    private bool IsInBounds(in NexusCoord pos) => Utility.IsInRange(pos, _playingFieldStart, _playingFieldEnd);

    private NexusCoord GetRandomFruit()
        => Utility.GetRandomCoord(_playingFieldStart + new NexusCoord(1, 1), _playingFieldEnd - new NexusCoord(1, 1));
}