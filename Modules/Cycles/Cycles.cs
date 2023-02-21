using System.Text.Json;
using System.Text.RegularExpressions;
using Discord.Interactions;
using WaframeDiscordBot.JSON;

namespace WaframeDiscordBot.Modules.Cycles;

[Group("cycle","Cycles")]
public partial class CyclesGroup : InteractionModuleBase<SocketInteractionContext>
{
    [GeneratedRegex("(?<hours>\\d+)h\\s?(?<minutes>\\d+)m\\s?(?<seconds>\\d+)s")]
    private static partial Regex TimeRegex();

    private static async Task<(CycleJson, long)> GetCycle(Globals.PlanetsEnum planet)
    {
        using var response = await Globals.HttpClient.GetAsync(Globals.PlanetsCycles[planet]);
        var content = await response.Content.ReadAsStringAsync();
        var cycle = JsonSerializer.Deserialize<CycleJson>(content);

        var match = TimeRegex().Match(cycle!.TimeLeft);
        var hours = int.Parse(match.Groups["hours"].Value);
        var minutes = int.Parse(match.Groups["minutes"].Value);
        var seconds = int.Parse(match.Groups["seconds"].Value);

        var timeSpan = new TimeSpan(hours, minutes, seconds);
        var unixTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (long)timeSpan.TotalSeconds;
        return (cycle, unixTimeStamp);
    }
}