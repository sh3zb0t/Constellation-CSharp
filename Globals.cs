using Discord;

namespace WaframeDiscordBot;

public class Globals
{
    public enum PlanetsEnum
    {
        Earth,
        Vallis,
        Cambion,
        Cetus
    }

    public static readonly HttpClient HttpClient = new();

    public static readonly RequestOptions Options = new()
    {
        Timeout = (int?)TimeSpan.FromSeconds(10).TotalSeconds,
        RetryMode = RetryMode.AlwaysRetry
    };

    public static readonly Dictionary<PlanetsEnum, string> PlanetsCycles = new()
    {
        { PlanetsEnum.Earth, "https://api.warframestat.us/pc/earthCycle/" },
        { PlanetsEnum.Vallis, "https://api.warframestat.us/pc/vallisCycle"},
        { PlanetsEnum.Cambion, "https://api.warframestat.us/pc/cambionCycle"},
        { PlanetsEnum.Cetus, "https://api.warframestat.us/pc/cetusCycle"}
    };

    // isStorm and isHard have to be false for normal list
    public const string FissureUrl = "https://api.warframestat.us/pc/fissures/";
}