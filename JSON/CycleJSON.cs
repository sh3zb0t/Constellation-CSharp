using System.Text.Json.Serialization;

namespace WaframeDiscordBot.JSON;

public abstract class CycleJson
{
    public class Root
    {
        public Root(string timeLeft)
        {
            TimeLeft = timeLeft;
        }

        [JsonPropertyName("timeLeft")]
        public string TimeLeft { get; set; }
    }
}