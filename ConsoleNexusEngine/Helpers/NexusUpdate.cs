﻿namespace ConsoleNexusEngine.Helpers;

/// <summary>
/// Useful methods for doing something in certain intervals
/// </summary>
public sealed class NexusUpdate
{
    private const double Second = 1;
    private const double Minute = 60;
    private const double Hour = 3600;

    private readonly double _intervalSeconds;
    private readonly Action _action;
    private double timeSince;

    private NexusUpdate(double intervalSeconds, Action action)
    {
        _intervalSeconds = intervalSeconds;
        _action = action;

        timeSince = 0;
    }

    /// <summary>
    /// Invokes <paramref name="action"/> every <paramref name="interval"/>
    /// </summary>
    /// <param name="interval">The interval to invoke <paramref name="action"/></param>
    /// <param name="action">The action to invoke</param>
    public static NexusUpdate DoEvery(in TimeSpan interval, Action action)
        => new NexusUpdate(interval.TotalSeconds, action);

    /// <summary>
    /// Invokes <paramref name="action"/> <paramref name="frequency"/> amount of times every <paramref name="interval"/>
    /// </summary>
    /// <param name="frequency">The amount of times in an <paramref name="interval"/></param>
    /// <param name="interval">The interval to invoke <paramref name="action"/></param>
    /// <param name="action">The action to invoke</param>
    public static NexusUpdate DoEvery(int frequency, in TimeSpan interval, Action action)
        => new NexusUpdate(interval.TotalSeconds / frequency, action);

    /// <summary>
    /// Invokes <paramref name="action"/> every second
    /// </summary>
    /// <param name="action">The action to invoke</param>
    public static NexusUpdate DoEverySecond(Action action)
        => new NexusUpdate(Second, action);

    /// <summary>
    /// Invokes <paramref name="action"/> <paramref name="frequency"/> amount of times every second
    /// </summary>
    /// <param name="frequency">The amount of times in a second</param>
    /// <param name="action">The action to invoke</param>
    public static NexusUpdate DoEverySecond(int frequency, Action action)
        => new NexusUpdate(Second / frequency, action);

    /// <summary>
    /// Invokes <paramref name="action"/> every minute
    /// </summary>
    /// <param name="action">The action to invoke</param>
    public static NexusUpdate DoEveryMinute(Action action)
        => new NexusUpdate(Minute, action);

    /// <summary>
    /// Invokes <paramref name="action"/> <paramref name="frequency"/> amount of times every minute
    /// </summary>
    /// <param name="frequency">The amount of times in a minute</param>
    /// <param name="action">The action to invoke</param>
    public static NexusUpdate DoEveryMinute(int frequency, Action action)
        => new NexusUpdate(Minute / frequency, action);

    /// <summary>
    /// Invokes <paramref name="action"/> every hour
    /// </summary>
    /// <param name="action">The action to invoke</param>
    public static NexusUpdate DoEveryHour(Action action)
        => new NexusUpdate(Hour, action);

    /// <summary>
    /// Invokes <paramref name="action"/> <paramref name="frequency"/> amount of times every hour
    /// </summary>
    /// <param name="frequency">The amount of times in an hour</param>
    /// <param name="action">The action to invoke</param>
    public static NexusUpdate DoEveryHour(int frequency, Action action)
        => new NexusUpdate(Hour / frequency, action);

    /// <summary>
    /// Call this inside <see cref="NexusConsoleGame.Update"/>
    /// </summary>
    /// <param name="deltaTime">Pass <see cref="NexusConsoleGame.DeltaTime"/> here</param>
    public void Update(double deltaTime)
    {
        timeSince += deltaTime;

        if (timeSince >= _intervalSeconds)
        {
            timeSince -= _intervalSeconds;

            _action();
        }
    }
}