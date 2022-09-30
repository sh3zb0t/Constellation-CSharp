using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;

namespace WaframeDiscordBot.Modules;

public class NormalCommandModule : InteractionModuleBase<SocketInteractionContext>
{
	private HttpClient Client { get; } = new();
	private RequestOptions Options { get; } = new()
	{
		Timeout = 3000,
		RetryMode = RetryMode.RetryRatelimit | RetryMode.Retry502 | RetryMode.RetryTimeouts,
		UseSystemClock = false
	};
	
	private static readonly Regex RedditRegex = new(@"^x[a-zA-Z0-9]{5}$");
	private static readonly Regex TwitterRegex = new(@"^1[45][0-9]{17}$");
	private static readonly Regex TikTokRegex = new(@"^[67][0-9]{18}$");

	[SlashCommand("cycles", "Displays the Cycle of all the planets")]
	public async Task Cycles()
	{
		
	}
}