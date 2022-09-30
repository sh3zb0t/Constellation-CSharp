using System.Text.Json.Serialization;

namespace WaframeDiscordBot.JSON;

public class EarthCycleJson
{
    public class Root
    {
        public Root(string id, DateTime expiry, DateTime activation, bool isDay, string state, string timeLeft)
        {
            Id = id;
            Expiry = expiry;
            Activation = activation;
            IsDay = isDay;
            State = state;
            TimeLeft = timeLeft;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("expiry")]
        public DateTime Expiry { get; set; }

        [JsonPropertyName("activation")]
        public DateTime Activation { get; set; }

        [JsonPropertyName("isDay")]
        public bool IsDay { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("timeLeft")]
        public string TimeLeft { get; set; }
    }
}