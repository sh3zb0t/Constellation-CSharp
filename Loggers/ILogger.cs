using Discord;

namespace WaframeDiscordBot.Loggers;

public interface ILogger
{
    // ReSharper disable once UnusedMemberInSuper.Global
    public Task Log(LogMessage message);
}