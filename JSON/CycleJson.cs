using System.Text.Json.Serialization;

namespace WaframeDiscordBot.JSON;

public class CycleJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("expiry")]
    public DateTime Expiry { get; set; }

    [JsonPropertyName("isWarm")]
    public bool? IsWarm { get; set; }

    [JsonPropertyName("isDay")]
    public bool IsDay { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("activation")]
    public DateTime Activation { get; set; }

    [JsonPropertyName("timeLeft")]
    public string TimeLeft { get; set; }

    [JsonPropertyName("shortString")]
    public string? ShortString { get; set; }
}