using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Serilog;
using WaframeDiscordBot.Modules;
using CyclesGroup = WaframeDiscordBot.Modules.Cycles.CyclesGroup;

namespace WaframeDiscordBot.Services;

public class InteractionHandler
{
    private readonly DiscordSocketClient _discordClient;
    private readonly InteractionService _commands;
    private readonly IServiceProvider _services;

    public InteractionHandler(DiscordSocketClient discordClient, InteractionService commands, IServiceProvider services)
    {
        _discordClient = discordClient;
        _commands = commands;
        _services = services;
    }

    public async Task InitializeAsync()
    {
        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        // Process the InteractionCreated payloads to execute Interactions commands
        _discordClient.InteractionCreated += HandleInteraction;

        // Process the command execution results
        _commands.SlashCommandExecuted += SlashCommandExecuted;
    }

    private static async Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, IResult arg3)
    {
        if (arg3 is { IsSuccess: false, Error: InteractionCommandError.UnmetPrecondition })
            await arg2.Interaction.RespondAsync(arg3.ErrorReason, ephemeral: true, options: Globals.Options);
    }

    private async Task HandleInteraction(SocketInteraction arg)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            SocketInteractionContext ctx = new (_discordClient, arg);
            await _commands.ExecuteCommandAsync(ctx, _services);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[{Source}] {Message}", ex.Source, ex.Message);

            // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist.
            // It is a good idea to delete the original response, or at least let the user know that something went wrong during the command execution.
            if (arg.Type is InteractionType.ApplicationCommand)
                await arg.GetOriginalResponseAsync()
                    .ContinueWith(async msg => await msg.Result.DeleteAsync());
        }
    }
}