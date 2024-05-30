namespace ConsoleNexusEngine.Helpers;

/// <summary>
/// Useful methods for doing something in certain intervals
/// </summary>
public static class NexusUpdate
{
    private const double Second = 1;
    private const double Minute = 60;
    private const double Hour = 3600;

    /// <summary>
    /// Invokes <paramref name="action"/> every <paramref name="interval"/>
    /// </summary>
    /// <param name="timeSince">The time since the last call</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    /// <param name="interval">The interval to invoke <paramref name="action"/></param>
    /// <param name="action">The action to invoke</param>
    public static void DoEvery(ref double timeSince, in double deltaTime, in TimeSpan interval, Action action)
        => DoEvery(ref timeSince, deltaTime, interval.TotalSeconds, action);

    /// <summary>
    /// Invokes <paramref name="action"/> <paramref name="frequency"/> amount of times every <paramref name="interval"/>
    /// </summary>
    /// <param name="timeSince">The time since the last call</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    /// <param name="frequency">The amount of times in an <paramref name="interval"/></param>
    /// <param name="interval">The interval to invoke <paramref name="action"/></param>
    /// <param name="action">The action to invoke</param>
    public static void DoEvery(ref double timeSince, in double deltaTime, in int frequency, in TimeSpan interval, Action action)
        => DoEvery(ref timeSince, deltaTime, interval.TotalSeconds / frequency, action);

    /// <summary>
    /// Invokes <paramref name="action"/> every second
    /// </summary>
    /// <param name="timeSince">The time since the last call</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    /// <param name="action">The action to invoke</param>
    public static void DoEverySecond(ref double timeSince, in double deltaTime, Action action)
        => DoEvery(ref timeSince, deltaTime, Second, action);

    /// <summary>
    /// Invokes <paramref name="action"/> <paramref name="frequency"/> amount of times every second
    /// </summary>
    /// <param name="timeSince">The time since the last call</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    /// <param name="frequency">The amount of times in a second</param>
    /// <param name="action">The action to invoke</param>
    public static void DoEverySecond(ref double timeSince, in double deltaTime, in int frequency, Action action)
        => DoEvery(ref timeSince, deltaTime, Second / frequency, action);

    /// <summary>
    /// Invokes <paramref name="action"/> every minute
    /// </summary>
    /// <param name="timeSince">The time since the last call</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    /// <param name="action">The action to invoke</param>
    public static void DoEveryMinute(ref double timeSince, in double deltaTime, Action action)
        => DoEvery(ref timeSince, deltaTime, Minute, action);

    /// <summary>
    /// Invokes <paramref name="action"/> <paramref name="frequency"/> amount of times every minute
    /// </summary>
    /// <param name="timeSince">The time since the last call</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    /// <param name="frequency">The amount of times in a minute</param>
    /// <param name="action">The action to invoke</param>
    public static void DoEveryMinute(ref double timeSince, in double deltaTime, in int frequency, Action action)
        => DoEvery(ref timeSince, deltaTime, Minute / frequency, action);

    /// <summary>
    /// Invokes <paramref name="action"/> every hour
    /// </summary>
    /// <param name="timeSince">The time since the last call</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    /// <param name="action">The action to invoke</param>
    public static void DoEveryHour(ref double timeSince, in double deltaTime, Action action)
        => DoEvery(ref timeSince, deltaTime, Hour, action);

    /// <summary>
    /// Invokes <paramref name="action"/> <paramref name="frequency"/> amount of times every hour
    /// </summary>
    /// <param name="timeSince">The time since the last call</param>
    /// <param name="deltaTime">The time elapsed since the last frame in total seconds</param>
    /// <param name="frequency">The amount of times in an hour</param>
    /// <param name="action">The action to invoke</param>
    public static void DoEveryHour(ref double timeSince, in double deltaTime, in int frequency, Action action)
        => DoEvery(ref timeSince, deltaTime, Hour / frequency, action);

    private static void DoEvery(ref double timeSince, in double deltaTime, in double secondsInterval, Action action)
    {
        timeSince += deltaTime;

        if (timeSince >= secondsInterval)
        {
            timeSince -= secondsInterval;

            action();
        }
    }
}