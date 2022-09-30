using System.Net.Http.Json;
using System.Reflection;
using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WaframeDiscordBot.JSON;

namespace WaframeDiscordBot.Services;
public class InteractionHandler
{
    private DiscordSocketClient DiscordClient { get; }
    private InteractionService Commands { get; }
    private IServiceProvider Services { get; }
    
    private RequestOptions Options { get; } = new()
    {
        RetryMode = RetryMode.AlwaysRetry | RetryMode.Retry502 | RetryMode.RetryRatelimit,
        Timeout = 10000
    };

    private HttpClient HttpClient { get; } = new();

    // Using constructor injection
    public InteractionHandler(DiscordSocketClient discordClient, InteractionService commands, IServiceProvider services)
    {
        DiscordClient = discordClient;
        Commands = commands;
        Services = services;
    }
    public async Task InitializeAsync()
    {
        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);

        // Process the InteractionCreated payloads to execute Interactions commands
        DiscordClient.InteractionCreated += HandleInteraction;

        // Process the command execution results 
        Commands.SlashCommandExecuted += SlashCommandExecuted;
        Commands.ContextCommandExecuted += ContextCommandExecuted;
        Commands.ComponentCommandExecuted += ComponentCommandExecuted;
    }

    private static Task ComponentCommandExecuted(ComponentCommandInfo arg1, IInteractionContext arg2, IResult arg3) => Task.CompletedTask;

    private static Task ContextCommandExecuted(ContextCommandInfo arg1, IInteractionContext arg2, IResult arg3) => Task.CompletedTask;

    private static Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3) => Task.CompletedTask;

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var ctx = new SocketInteractionContext(DiscordClient, arg);
            await Commands.ExecuteCommandAsync(ctx, Services);
        } catch (Exception ex) {
            Console.WriteLine(ex);

            // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist.
            // It is a good idea to delete the original response, or at least let the user know that something went wrong during the command execution.
            if (arg.Type == InteractionType.ApplicationCommand)
                await arg.GetOriginalResponseAsync()
                    .ContinueWith(async msg => await msg.Result.DeleteAsync());
        }
    }
}