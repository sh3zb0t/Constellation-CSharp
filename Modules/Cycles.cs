using System.Text.RegularExpressions;
using Discord.Interactions;

namespace WaframeDiscordBot.Modules;

public sealed partial class TopLevel
{
    [SlashCommand("cycles", "Displays the Cycle of all the planets")]
    public async Task CyclesCmd()
    {

    }

    [GeneratedRegex("(\\d+)h (\\d+)m (\\d+)s")]
    private static partial Regex HoursRegex();

    [GeneratedRegex("(\\d+)m (\\d+)s")]
    private static partial Regex MinutesRegex();

    [GeneratedRegex("(\\d+)s")]
    private static partial Regex SecondsRegex();
}