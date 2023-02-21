using Discord;
using Discord.Interactions;

namespace WaframeDiscordBot.Modules;

public sealed partial class TopLevel : InteractionModuleBase<SocketInteractionContext>
{
    public static readonly HttpClient HttpClient = new();

    public static readonly RequestOptions Options = new()
    {
        Timeout = 8000,
        RetryMode = RetryMode.AlwaysRetry
    };
}