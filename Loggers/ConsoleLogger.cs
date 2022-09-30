using Discord;

namespace WaframeDiscordBot.Loggers;

public class ConsoleLogger : Logger
{
    public override Task Log(LogMessage message)
    {
        // Using Task.Run() in case there are any long running actions, to prevent blocking gateway
        Task.Run(() => LogToConsoleAsync(message));
        return Task.CompletedTask;
    }

    private static Task LogToConsoleAsync(LogMessage message)
    {
        Console.WriteLine($"{message} ");
        return Task.CompletedTask;
    }
}