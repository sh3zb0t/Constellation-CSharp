using Discord;

namespace WaframeDiscordBot.Loggers;

public abstract class Logger : ILogger
{
    public abstract Task Log(LogMessage message);
}