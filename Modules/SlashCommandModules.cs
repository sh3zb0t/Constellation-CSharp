using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Newtonsoft.Json;
using WaframeDiscordBot.JSON;

namespace WaframeDiscordBot.Modules;

public class NormalCommandModule : InteractionModuleBase<SocketInteractionContext>
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
		var cetusCycleJsonResponse = await HttpClient.GetFromJsonAsync<CetusCycleJSON.Root>("https://api.warframestat.us/pc/cetusCycle");
		var cetusCycleTimeLeft = cetusCycleJsonResponse?.TimeLeft;
		
		int minutes;
		int seconds;
		var timeLeft = 0;
	
		var hoursMatch = new Regex(@"^(-?)([0-9]{1,2}h) ([0-9]{1,2}m) ([0-9]{1,2}s)$").Match(cetusCycleTimeLeft!);
		var minutesMatch = new Regex(@"^(-?)([0-9]{1,2}m) ([0-9]{1,2}s)$").Match(cetusCycleTimeLeft!);
		var secondsMatch = new Regex(@"^(-?)([0-9]{1,2}s)$").Match(cetusCycleTimeLeft!);
	
		if (hoursMatch.Success)
		{
			var hours = int.Parse(hoursMatch.Groups[2].Value.Replace("h", ""));
			minutes = int.Parse(hoursMatch.Groups[3].Value.Replace("m", ""));
			seconds = int.Parse(hoursMatch.Groups[4].Value.Replace("s", ""));
			timeLeft = hours * 60 * 60 + minutes * 60 + seconds;
		}
		else if (minutesMatch.Success)
		{
			minutes = int.Parse(minutesMatch.Groups[2].Value.Replace("m", ""));
			seconds = int.Parse(minutesMatch.Groups[3].Value.Replace("s", ""));
			timeLeft = minutes * 60 + seconds;
		}
		else if (secondsMatch.Success)
		{
			seconds = int.Parse(secondsMatch.Groups[2].Value.Replace("s", ""));
			timeLeft = seconds;
		}
		else
		{
			await FollowupAsync("Something went wrong", ephemeral: true, options: Options);
		}
		var timeLeftInUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + timeLeft;
		
		await FollowupAsync(embed: new EmbedBuilder()
			.WithTitle("Earth Cycle")
			.WithDescription(
				$"Earth Cycle will end **<t:{timeLeftInUnix}:R>**")
			.WithColor(Color.Blue)
			.Build(), options: Options);
	}

	[SlashCommand("fissures", "Displays the fissures")]
	public async Task Fissures()
	{
		await DeferAsync();
		
		var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToLower().Contains("ping"));

		var fissuresJsonResponse = await HttpClient.GetStringAsync("https://api.warframestat.us/pc/fissures");
		var fissuresDeserializeObject = JsonConvert.DeserializeObject<List<FissuresJson.Root>>(fissuresJsonResponse);

		var planets = new List<string>();
		var missionTypes = new List<string>();
		var enemyTypes = new List<string>();
		var expiryTimes = new List<string>();
		var types = new List<string>();
		var isHard = new List<bool>();
		var isStorm = new List<bool>();
		

		foreach (var fissure in fissuresDeserializeObject!)
		{
			var timeLeftInUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds() +
			                     (int)(fissure.Expiry - DateTimeOffset.UtcNow).TotalSeconds;
			planets.Add(fissure.Node);
			missionTypes.Add(fissure.MissionType);
			enemyTypes.Add(fissure.Enemy);
			expiryTimes.Add($"<t:{timeLeftInUnix}:R>");
			types.Add(fissure.Tier);
			isHard.Add(fissure.IsHard);
			isStorm.Add(fissure.IsHard);
		}

		var embedBuilder = new EmbedBuilder()
			.WithTitle("Fissures")
			.WithColor(Color.Blue);
		EmbedBuilder steelPathEmbedBuilder;
		EmbedBuilder stormEmbedBuilder;

		int stormCount;
		int steelPathCount;
		switch (fissuresDeserializeObject.Count)
		{
			case <= 25:
				embedBuilder = new EmbedBuilder()
					.WithTitle("Fissures")
					.WithColor(Color.Blue);
		
				for (var i = 0; i < fissuresDeserializeObject.Count; i++)
				{
					
					embedBuilder.AddField($"Fissure {i + 1}", $"Place: {planets[i]}\n" +
					                                          $"Mission Type: {missionTypes[i]}\n" +
					                                          $"Enemy: {enemyTypes[i]}\n" +
					                                          $"Expiry: {expiryTimes[i]}\n" +
					                                          $"Type: {types[i]}\n");
				}

				steelPathCount = fissuresDeserializeObject.Count(fissure => fissure.IsHard);
				stormCount = fissuresDeserializeObject.Count(fissure => fissure.IsStorm);
				
				steelPathEmbedBuilder = new EmbedBuilder()
					.WithTitle($"They are {steelPathCount} Steel Path Missions")
					.WithColor(Color.Blue);
				
				stormEmbedBuilder = new EmbedBuilder()
					.WithTitle($"They are {stormCount} Storm Missions")
					.WithColor(Color.Blue);
				
				foreach (var embed in new[] {embedBuilder, steelPathEmbedBuilder, stormEmbedBuilder})
				{
					await FollowupAsync(embed: embed.Build(), options: Options);
				}
				
				break;
			case >= 26:
			{
				for (var i = 0; i < fissuresDeserializeObject.Count; i++)
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
					
					if (i % 25 == 24 || i == fissuresDeserializeObject.Count - 1)
					{
						await FollowupAsync(embed: embedBuilder.Build(), options: Options);
					}

				}
				
				steelPathCount = fissuresDeserializeObject.Count(fissure => fissure.IsHard);
				stormCount = fissuresDeserializeObject.Count(fissure => fissure.IsStorm);
				
				steelPathEmbedBuilder = new EmbedBuilder()
					.WithTitle($"They are {steelPathCount} Steel Path Missions")
					.WithColor(Color.Blue);
				
				stormEmbedBuilder = new EmbedBuilder()
					.WithTitle($"They are {stormCount} Storm Missions")
					.WithColor(Color.Blue);

				foreach (var embed in new[] {steelPathEmbedBuilder, stormEmbedBuilder})
				{
					await FollowupAsync(embed: embed.Build(), options: Options);
				}
				
				break;
			}
		}

		await ReplyAsync(role?.Mention);
	}
}