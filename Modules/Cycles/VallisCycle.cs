using Discord;
using Discord.Interactions;

namespace WaframeDiscordBot.Modules.Cycles;

public partial class CyclesGroup
{
    [SlashCommand("vallis","Vallis Cycle")]
    public async Task VallisCycleCmd()
    {
        await DeferAsync(options: Globals.Options);

        var cycle = await GetCycle(Globals.PlanetsEnum.Earth);

        var embed = new EmbedBuilder()
            .WithTitle("Vallis Cycle")
            .WithDescription($"**State:** {cycle.Item1.State}\n**Next Cycle:** <t:{cycle.Item2}:R>")
            .WithColor(cycle.Item1.IsDay ? Color.Blue : Color.DarkBlue)
            .WithFooter("Data provided by warframestat.us")
            .Build();

        await FollowupAsync(embed: embed, options: Globals.Options);
    }
}