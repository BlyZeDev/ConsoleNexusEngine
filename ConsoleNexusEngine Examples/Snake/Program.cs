namespace Snake;

using ConsoleNexusEngine.Helpers;

sealed class Program
{
    static void Main()
    {
        var directory = Environment.CurrentDirectory;

        if (!NexusEngineHelper.IsSupportedConsole())
        {
            NexusEngineHelper.StartInSupportedConsole();
            return;
        }

        ConsoleKey key;
        do
        {
            using (var game = new SnakeGame(directory))
            {
                game.Start();
                game.Stop();
            }

            key = Console.ReadKey(true).Key;

        } while (key != ConsoleKey.Escape);
    }
}
