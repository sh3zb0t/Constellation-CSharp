using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using WaframeDiscordBot.JSON;

namespace WaframeDiscordBot.Modules;

public sealed class NormalCommandModule : InteractionModuleBase<SocketInteractionContext>
{
	private HttpClient HttpClient { get; } = new();

	private RequestOptions Options { get; } = new()
	{
		Timeout = 3000,
		RetryMode = RetryMode.RetryRatelimit | RetryMode.Retry502 | RetryMode.RetryTimeouts,
		UseSystemClock = false
	};

	[SlashCommand("cycles", "Displays the Cycle of all the planets")]
	public async Task Cycles()
	{
		await DeferAsync(options: Options);
		
		var cycleEnds = new Dictionary<string, long>();
		
		foreach (var cycle in new[] {"earth", "cetus", "cambion", "zariman", "vallis"})
		{
			var json = await HttpClient.GetFromJsonAsync<CycleJson.Root>($"https://api.warframestat.us/pc/{cycle}Cycle");
			var cycleHoursMatch = Regex.Match(json!.TimeLeft, @"(\d+)h (\d+)m (\d+)s");
			var cycleMinutesMatch = Regex.Match(json.TimeLeft, @"(\d+)m (\d+)s");
			var cycleSecondsMatch = Regex.Match(json.TimeLeft, @"(\d+)s");

			var timeLeft = cycleHoursMatch.Success switch
			{
				true => int.Parse(cycleHoursMatch.Groups[1].Value) * 60 * 60 + int.Parse(cycleHoursMatch.Groups[2].Value) * 60 +
				        int.Parse(cycleHoursMatch.Groups[3].Value),
				false when cycleMinutesMatch.Success => int.Parse(cycleMinutesMatch.Groups[1].Value) * 60 + int.Parse(cycleMinutesMatch.Groups[2].Value),
				false when cycleSecondsMatch.Success => int.Parse(cycleSecondsMatch.Groups[1].Value),
				_ => 0
			};
		
			cycleEnds.Add(cycle, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + timeLeft);
		}
		
		await FollowupAsync(embeds: cycleEnds.Select(cycle => new EmbedBuilder()
			.WithTitle($"{cycle.Key.First().ToString().ToUpper()}{cycle.Key[1..]} Cycle")
			.WithDescription(
				$"{cycle.Key.First().ToString().ToUpper()}{cycle.Key[1..]} Cycle will end** <t:{cycle.Value}:R>**")
			.WithColor(Color.Blue)
			.Build()).ToArray(), options: Options);
	}

	[SlashCommand("fissures", "Displays the fissures")]
	public async Task Fissures()
	{
		await DeferAsync();
		
		var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Contains("ping"));
		var fissuresJsonResponse = await HttpClient.GetFromJsonAsync<List<FissuresJson.Root>>("https://api.warframestat.us/pc/fissures");
		
		var planets = fissuresJsonResponse!.Select(x => x.Node).ToList();
		var missionTypes = fissuresJsonResponse!.Select(x => x.MissionType).ToList();
		var enemyTypes = fissuresJsonResponse!.Select(x => x.Enemy).ToList();
		var expiryTimes = fissuresJsonResponse!.Select(x => x.Expiry).ToList(); 
		var types = fissuresJsonResponse!.Select(x => x.Tier).ToList();

		var embedBuilder = new EmbedBuilder()
			.WithTitle("Fissures")
			.WithColor(Color.Blue);

		for (var i = 0; i < fissuresJsonResponse!.Count; i++)
		{
			if (i % 25 == 0)
			{
				embedBuilder = new EmbedBuilder()
					.WithTitle("Fissures")
					.WithColor(Color.Blue);
			}
			
			embedBuilder.AddField($"Fissure {i + 1}", $"Place: {planets[i]}\n" + 
			                                          $"Mission Type: {missionTypes[i]}\n" + 
			                                          $"Enemy: {enemyTypes[i]}\n" + 
			                                          $"Expiry: {expiryTimes[i]}\n" + 
			                                          $"Type: {types[i]}\n");
			
			if (i % 25 == 24 || i == fissuresJsonResponse.Count - 1)
			{
				await FollowupAsync(embed: embedBuilder.Build(), options: Options);
			}

		}
		
		foreach (var embed in new[] {new EmbedBuilder()
			         .WithTitle($"They are {fissuresJsonResponse.Count(fissure => fissure.IsHard)} Steel Path Missions")
			         .WithColor(Color.Blue), new EmbedBuilder()
			         .WithTitle($"They are {fissuresJsonResponse.Count(fissure => fissure.IsStorm)} Storm Missions")
			         .WithColor(Color.Blue)})
		{
			await FollowupAsync(embed: embed.Build(), options: Options);
		}

		await ReplyAsync(role?.Mention);
	}
}