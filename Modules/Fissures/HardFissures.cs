using System.Net.Http.Json;
using Discord;
using Discord.Interactions;
using WaframeDiscordBot.JSON;

namespace WaframeDiscordBot.Modules.Fissures;

public sealed partial class FissuresGroup
{
    public enum SortBy
    {
        Tier,
        Node,
        MissionType,
        Enemy
    }

    [SlashCommand("hard", "Hard Fissueres")]
    public async Task HardFissures([Choice("Tier", (int)SortBy.Tier)]
                                   [Choice("Node", (int)SortBy.Node)]
                                   [Choice("MissionType", (int)SortBy.MissionType)]
                                   [Choice("Enemy", (int)SortBy.Enemy)]
                                   SortBy sortBy = SortBy.Tier)
    {
        await DeferAsync(options: Globals.Options);

        var response = await Globals.HttpClient.GetFromJsonAsync<List<FissuresJson>>(Globals.FissureUrl);
        if (response is null)
        {
            await RespondAsync("Failed to fetch fissures.", options: Globals.Options);
            return;
        }

        var hardFissures = response.Where(x => x.IsHard).ToList();
        hardFissures.Sort((f1, f2) => sortBy switch
        {
            SortBy.Tier => string.Compare(f1.Tier, f2.Tier, StringComparison.OrdinalIgnoreCase),
            SortBy.Node => string.Compare(f1.Node, f2.Node, StringComparison.OrdinalIgnoreCase),
            SortBy.MissionType => string.Compare(f1.MissionType, f2.MissionType, StringComparison.OrdinalIgnoreCase),
            SortBy.Enemy => string.Compare(f1.Enemy, f2.Enemy, StringComparison.OrdinalIgnoreCase),
            _ => throw new ArgumentException($"Invalid sort by option {sortBy}.", nameof(sortBy))
        });

        Dictionary<string, List<FissuresJson>> groupFissures = new();
        foreach (var fissure in hardFissures)
        {
            if (!groupFissures.TryGetValue(GetGroupByProperty(sortBy, fissure), out var fissures))
            {
                fissures = new List<FissuresJson>();
                groupFissures.Add(GetGroupByProperty(sortBy, fissure), fissures);
            }
            fissures.Add(fissure);
        }

        List<EmbedBuilder> embeds = new();
        foreach (var (groupValue, fissures) in groupFissures)
        {
            var embed = new EmbedBuilder()
                .WithTitle($"{sortBy}: {groupValue}")
                .WithColor(new Color(Random.Shared.Next(0, 255), Random.Shared.Next(0, 255), Random.Shared.Next(0, 255)))
                .WithFooter("Data provided by warframestat.us")
                .WithCurrentTimestamp();
            foreach (var fissure in fissures)
            {
                var fieldName = GetFieldName(sortBy, fissure);
                var fieldValue = GetFieldValue(sortBy, fissure);
                embed.AddField(fieldName, fieldValue);
            }
            embeds.Add(embed);
        }

        await FollowupAsync(embeds: embeds.Select(x => x.Build()).ToArray(), options: Globals.Options);
    }

    private static string GetGroupByProperty(SortBy sortBy, FissuresJson fissure)
    {
        return sortBy switch
        {
            SortBy.Tier => fissure.Tier,
            SortBy.Node => fissure.Node,
            SortBy.MissionType => fissure.MissionType,
            SortBy.Enemy => fissure.Enemy,
            _ => throw new ArgumentException($"Invalid sort by option {sortBy}.", nameof(sortBy))
        };
    }

    private static string GetFieldName(SortBy sortBy, FissuresJson fissure)
    {
        var propertyName = sortBy switch
        {
            SortBy.Tier => fissure.Tier,
            SortBy.Node => fissure.Node,
            SortBy.MissionType => fissure.MissionType,
            SortBy.Enemy => fissure.Enemy,
            _ => throw new ArgumentException($"Invalid sort by option {sortBy}.", nameof(sortBy))
        };

        if (sortBy is SortBy.MissionType && propertyName is "Fissures")
            return fissure.Node;

        return propertyName;
    }

    private static string GetFieldValue(SortBy sortBy, FissuresJson fissure)
    {
        return sortBy switch
        {
            SortBy.Tier => $"**Mission Type:** {fissure.MissionType}\n**Node:** {fissure.Node}\n**Enemy:** {fissure.Enemy}",
            SortBy.Node => $"**Mission Type:** {fissure.MissionType}\n**Tier:** {fissure.Tier}\n**Enemy:** {fissure.Enemy}",
            SortBy.MissionType => $"**Tier:** {fissure.Tier}\nNode: {fissure.Node}\n**Enemy:** {fissure.Enemy}",
            SortBy.Enemy => $"**Tier:** {fissure.Tier}\n**Node:** {fissure.Node}\nMission Type:** {fissure.MissionType}",
            _ => throw new ArgumentException($"Invalid sort by option {sortBy}.", nameof(sortBy))
        };
    }

}