using Discord;
using Discord.Interactions;
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

	// [SlashCommand("cycles", "Displays the Cycle of all the planets")]
	// public async Task Cycles()
	// {
	// 	await DeferAsync(options: Options);
	// 	var cetusCycleJsonResponse = await HttpClient.GetFromJsonAsync<CetusCycleJSON.Root>("https://api.warframestat.us/pc/cetusCycle");
	// 	var cetusCycleTimeLeft = cetusCycleJsonResponse?.TimeLeft;
	// 	Console.WriteLine(cetusCycleTimeLeft);
	//
	// 	int minutes;
	// 	int seconds;
	// 	var timeLeft = 0;
	//
	// 	var hoursMatch = new Regex(@"^(-?)([0-9]{1,2}h) ([0-9]{1,2}m) ([0-9]{1,2}s)$").Match(cetusCycleTimeLeft!);
	// 	var minutesMatch = new Regex(@"^(-?)([0-9]{1,2}m) ([0-9]{1,2}s)$").Match(cetusCycleTimeLeft!);
	// 	var secondsMatch = new Regex(@"^(-?)([0-9]{1,2}s)$").Match(cetusCycleTimeLeft!);
	//
	// 	if (hoursMatch.Success)
	// 	{
	// 		var hours = int.Parse(hoursMatch.Groups[2].Value.Replace("h", ""));
	// 		minutes = int.Parse(hoursMatch.Groups[3].Value.Replace("m", ""));
	// 		seconds = int.Parse(hoursMatch.Groups[4].Value.Replace("s", ""));
	// 		timeLeft = hours * 60 * 60 + minutes * 60 + seconds;
	// 	}
	// 	else if (minutesMatch.Success)
	// 	{
	// 		minutes = int.Parse(minutesMatch.Groups[2].Value.Replace("m", ""));
	// 		seconds = int.Parse(minutesMatch.Groups[3].Value.Replace("s", ""));
	// 		timeLeft = minutes * 60 + seconds;
	// 	}
	// 	else if (secondsMatch.Success)
	// 	{
	// 		seconds = int.Parse(secondsMatch.Groups[2].Value.Replace("s", ""));
	// 		timeLeft = seconds;
	// 	}
	// 	else
	// 	{
	// 		await FollowupAsync("Something went wrong", ephemeral: true, options: Options);
	// 	}
	// 	
	// 	var timeLeftString = new DateTime().AddSeconds(timeLeft).ToString("HH:mm:ss");
	// 	
	// 	var earthTimeMessage = await FollowupAsync(embed: new EmbedBuilder()
	// 		.WithTitle("Earth Cycle")
	// 		.WithDescription(
	// 			$"**Remaining Time:** {timeLeftString}")
	// 		.WithColor(Color.Blue)
	// 		.Build(), components: new ComponentBuilder().WithButton("Stop Countdown", "stop_countdown", ButtonStyle.Danger).Build(), options: Options);
	//
	// 	SocketMessageComponent socketMessageComponent = null;
	// 	
	// 	for (var i = timeLeft; i > 0; i--)
	// 	{
	// 		var newTimeLeft = new DateTime().AddSeconds(i).ToString("HH:mm:ss");
	// 		var newEmbed = new EmbedBuilder()
	// 			.WithTitle("Earth Cycle")
	// 			.WithDescription(
	// 				$"**Remaining Time:** {newTimeLeft}")
	// 			.WithColor(Color.Blue)
	// 			.Build();
	// 		await earthTimeMessage.ModifyAsync(x => x.Embed = newEmbed, options: Options);
	// 		// Context.Client.ButtonExecuted += async arg =>
	// 		// {
	// 		// 	if (arg.Data.CustomId == "stop_countdown")
	// 		// 	{
	// 		// 		await arg.ModifyOriginalResponseAsync(x => x.Components = new ComponentBuilder().WithButton("Start Countdown", "start_countdown", ButtonStyle.Success).Build(), options: Options);
	// 		// 		await arg.DeferLoadingAsync();
	// 		// 	}
	// 		// };
	// 		await Task.Delay(1000);
	// 		if(socketMessageComponent?.Data.CustomId == "stop_countdown")
	// 			break;
	// 	}
	// }

	[SlashCommand("fissures", "Displays the fissures")]
	public async Task Fissures()
	{
		await DeferAsync();

		var fissuresJsonResponse = await HttpClient.GetStringAsync("https://api.warframestat.us/pc/fissures");
		var fissuresDeserializeObject = JsonConvert.DeserializeObject<List<FissuresJson.Root>>(fissuresJsonResponse);

		foreach (var root in fissuresDeserializeObject)
		{
			var expiry = root.Expiry;
			var missionType = root.MissionType;
			var enemy = root.Enemy;
			
			if (missionType == "Survival")
			{
				var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.Contains("pinged"));
			}
		}
	}
}