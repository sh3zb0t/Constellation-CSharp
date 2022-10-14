using System.Globalization;
using System.Net.Http.Json;
using System.Reflection;
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
        RetryMode = RetryMode.AlwaysRetry, // Always retry has 502, RateLimit and Timeouts included
        Timeout = 5000
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
        DiscordClient.Ready += () =>
        {
            _ = HandleCheck();
            return Task.CompletedTask;
        };

        // Process the command execution results 
        Commands.SlashCommandExecuted += SlashCommandExecuted;
        Commands.ContextCommandExecuted += ContextCommandExecuted;
        Commands.ComponentCommandExecuted += ComponentCommandExecuted;
    }

    private async Task HandleCheck()
    {
        await NewFissuresCheck();
    }

    private async Task NewFissuresCheck()
    {
        while (true)
        {
            var channel = DiscordClient.Guilds.SelectMany(x => x.Channels)
                .FirstOrDefault(x => x.Name == "fissures") as SocketTextChannel;

            var role = DiscordClient.Guilds.SelectMany(x => x.Roles)
                .FirstOrDefault(x => x.Name.ToLower().Contains("fissures"));

            var fissuresJsonResponse =
                await HttpClient.GetFromJsonAsync<List<FissuresJson.Root>>("https://api.warframestat.us/pc/fissures");

            var embedBuilder = new EmbedBuilder()
                .WithTitle("Fissures")
                .WithColor(Color.Blue);

            var results = new List<string?>();
            var results2 = new List<string>();

            results.AddRange(fissuresJsonResponse!.Where(x => x.IsHard).Select(x => x.Node));

            await Task.Delay(TimeSpan.FromMinutes(10));

            var fissuresJsonResponse2 =
                await HttpClient.GetFromJsonAsync<List<FissuresJson.Root>>("https://api.warframestat.us/pc/fissures");
            var newMissionTypes = fissuresJsonResponse2!.Select(x => x.MissionType).ToList();
            var newIsStorm = fissuresJsonResponse2!.Select(x => x.IsStorm).ToList();
            var newIsHard = fissuresJsonResponse2!.Select(x => x.IsHard).ToList();
            var newNode = fissuresJsonResponse2!.Select(x => x.Node).ToList();

            results2.AddRange(fissuresJsonResponse2!.Where(x => x.IsHard).Select(x => x.Node));

            if (results.SequenceEqual(results2)) return;

            var survivalMissions = new List<string>();
            var normalMission = new List<string>();
            var isHardList = new List<string>();
            var isStormList = new List<string>();

            for (var i = 0; i < fissuresJsonResponse2!.Count; i++)
            {
                if (!fissuresJsonResponse2[i].Active) break;

                double unixTimeStamp;
                DateTime localTime;
                DateTime date;

                if (newMissionTypes[i] == "Survival")
                {
                    date = DateTime.Parse(fissuresJsonResponse2[i].Expiry.ToString(CultureInfo.InvariantCulture));
                    localTime = TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.Local);
                    unixTimeStamp = localTime.ToUniversalTime()
                        .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                    survivalMissions.Add($"**Type:** {newMissionTypes[i]}\n**Node:** {newNode[i]}\n<t:{unixTimeStamp}:R>");
                    // Console.WriteLine($"**Type:** {newMissionTypes[i]}\n**Node:** {newNode[i]}\n<t:{unixTimeStamp}:R>");
                }

                foreach (var mission in newMissionTypes)
                {
                    date = DateTime.Parse(fissuresJsonResponse2[i].Expiry.ToString(CultureInfo.InvariantCulture));
                    localTime = TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.Local);
                    unixTimeStamp = localTime.ToUniversalTime()
                        .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                    normalMission.Add($"**Type:** {mission}\n**Node:** {newNode[i]}\n<t:{unixTimeStamp}:R>");
                    // Console.WriteLine($"**Type:** {newMissionTypes[i]}\n**Node:** {newNode[i]}\n<t:{unixTimeStamp}:R>");
                }

                if (newIsHard[i])
                {
                    date = DateTime.Parse(fissuresJsonResponse2[i].Expiry.ToString(CultureInfo.InvariantCulture));
                    localTime = TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.Local);
                    unixTimeStamp = localTime.ToUniversalTime()
                        .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                    isHardList.Add($"**Type:** {newMissionTypes[i]}\n**Node:** {newNode[i]}\n<t:{unixTimeStamp}:R>");
                    // Console.WriteLine($"**Type:** {newMissionTypes[i]}\n**Node:** {newNode[i]}\n<t:{unixTimeStamp}:R>");
                }

                if (!newIsStorm[i]) continue;
                date = DateTime.Parse(fissuresJsonResponse2[i].Expiry.ToString(CultureInfo.InvariantCulture));
                localTime = TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.Local);
                unixTimeStamp = localTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                    .TotalSeconds;
                isStormList.Add($"**Type:** {newMissionTypes[i]}\n**Node:** {newNode[i]}\n<t:{unixTimeStamp}:R>");
                // Console.WriteLine($"**Type:** {newMissionTypes[i]}\n**Node:** {newNode[i]}\n<t:{unixTimeStamp}:R>");
            }

            if (isStormList.Count == 0) isStormList.Add("No Storm missions");
            if (isHardList.Count == 0) isHardList.Add("No Hard missions");
            if (normalMission.Count == 0) normalMission.Add("No Normal missions");
            if (survivalMissions.Count == 0) survivalMissions.Add("No Survival missions");

            embedBuilder.AddField("Void Storm", string.Join("\n", isStormList), true);
            embedBuilder.AddField("Steel Path", string.Join("\n", isHardList), true);
            embedBuilder.AddField("Survival Missions:", string.Join("\n", survivalMissions), true);
            // embedBuilder.AddField("Normal Missions:", string.Join("\n", normalMission), true);

            await channel!.SendMessageAsync($"<@&{role}>", embed: embedBuilder.Build(), options: Options);

            results.Clear();
            results2.Clear();
        }
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