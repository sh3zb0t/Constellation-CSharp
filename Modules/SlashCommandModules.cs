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
		
		var earthCycleTimeLeft = (await HttpClient.GetFromJsonAsync<EarthCycleJson.Root>("https://api.warframestat.us/pc/earthCycle"))?.TimeLeft;
		var cetusCycleTimeLeft = (await HttpClient.GetFromJsonAsync<CetusCycleJSON.Root>("https://api.warframestat.us/pc/cetusCycle"))?.TimeLeft;
		
		Regex hoursMatch = new(@"^(-?)([0-9]{1,2}h) ([0-9]{1,2}m) ([0-9]{1,2}s)$");
		var earthCycleHoursMatch = hoursMatch.Match(earthCycleTimeLeft!);
		var cetusCycleHoursMatch = hoursMatch.Match(cetusCycleTimeLeft!);
		
		Regex minutesMatch = new(@"^(-?)([0-9]{1,2}m) ([0-9]{1,2}s)$");
		var earthCycleMinutesMatch = minutesMatch.Match(earthCycleTimeLeft!);
		var cetusCycleMinutesMatch = minutesMatch.Match(cetusCycleTimeLeft!);
		
		Regex secondsMatch = new(@"^(-?)([0-9]{1,2}s)$");
		var earthCycleSecondsMatch = secondsMatch.Match(earthCycleTimeLeft!);
		var cetusCycleSecondsMatch = secondsMatch.Match(cetusCycleTimeLeft!);

		var earthTimeLeft = earthCycleHoursMatch.Success switch
		{
			true => int.Parse(earthCycleHoursMatch.Groups[2].Value.Replace("h", "")) * 60 * 60 + int.Parse(earthCycleHoursMatch.Groups[3].Value.Replace("m", "")) * 60 +
			        int.Parse(earthCycleHoursMatch.Groups[4].Value.Replace("s", "")),
			false when earthCycleMinutesMatch.Success => int.Parse(earthCycleMinutesMatch.Groups[2].Value.Replace("m", "")) * 60 + int.Parse(earthCycleMinutesMatch.Groups[3].Value.Replace("s", "")),
			false when earthCycleSecondsMatch.Success => int.Parse(earthCycleSecondsMatch.Groups[2].Value.Replace("s", "")),
			_ => 0
		};
		
		var cetusTimeLeft = cetusCycleHoursMatch.Success switch
		{
			true => int.Parse(cetusCycleHoursMatch.Groups[2].Value.Replace("h", "")) * 60 * 60 + int.Parse(cetusCycleHoursMatch.Groups[3].Value.Replace("m", "")) * 60 +
			        int.Parse(cetusCycleHoursMatch.Groups[4].Value.Replace("s", "")),
			false when cetusCycleMinutesMatch.Success => int.Parse(cetusCycleMinutesMatch.Groups[2].Value.Replace("m", "")) * 60 + int.Parse(cetusCycleMinutesMatch.Groups[3].Value.Replace("s", "")),
			false when cetusCycleSecondsMatch.Success => int.Parse(cetusCycleSecondsMatch.Groups[2].Value.Replace("s", "")),
			_ => 0
		};
		
		var earthCycleEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + earthTimeLeft;
		var cetusCycleEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + cetusTimeLeft;

		await FollowupAsync(embeds: new[]
		{
			new EmbedBuilder()
				.WithTitle("Earth Cycle")
				.WithDescription(
					$"Earth Cycle will end **<t:{earthCycleEnd}:R>**")
				.WithColor(Color.Blue)
				.Build(),
			new EmbedBuilder()
				.WithTitle("Cetus Cycle")
				.WithDescription(
					$"Cetus Cycle will end **<t:{cetusCycleEnd}:R>**")
				.WithColor(Color.Blue)
				.Build()
		}, options: Options);
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