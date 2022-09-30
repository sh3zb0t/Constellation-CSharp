using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using WaframeDiscordBot.Services;
using WaframeDiscordBot.Loggers;
using static System.Environment;
using static Microsoft.Extensions.Hosting.Host;

var host = CreateDefaultBuilder()
    .ConfigureServices((_,
        services) => services.AddSingleton(_ => new DiscordSocketClient(new DiscordSocketConfig { 
            GatewayIntents = GatewayIntents.AllUnprivileged & ~GatewayIntents.GuildScheduledEvents & ~GatewayIntents.GuildInvites | GatewayIntents.MessageContent,
            AlwaysDownloadUsers = true,
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            LogLevel = LogSeverity.Warning | LogSeverity.Error,
            UseSystemClock = true,
            MessageCacheSize = 100
        }))
        .AddTransient<ConsoleLogger>()
        .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
        .AddSingleton<InteractionHandler>()
        .AddSingleton(_ => new CommandService(new CommandServiceConfig {
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            LogLevel = LogSeverity.Info | LogSeverity.Error,
            DefaultRunMode = Discord.Commands.RunMode.Async
        })))
    .Build();

using var serviceScope = host.Services.CreateScope();
var provider = serviceScope.ServiceProvider;
var commands = provider.GetRequiredService<InteractionService>();
var client = provider.GetRequiredService<DiscordSocketClient>();
await provider.GetRequiredService<InteractionHandler>().InitializeAsync();

client.Log += _ => provider.GetRequiredService<ConsoleLogger>().Log(_);
commands.Log += _ => provider.GetRequiredService<ConsoleLogger>().Log(_);

// Registers commands globally
// client.Ready += async () => await commands.RegisterCommandsGloballyAsync();

await client.LoginAsync(TokenType.Bot, GetEnvironmentVariable("WARFRAME_DISCORD_BOT_TOKEN"));
await client.StartAsync();
await Task.Delay(-1);