using System.Text.Json.Serialization;

namespace WaframeDiscordBot.JSON;

public class CetusCycleJSON
{
    public class Root
    {
        public Root(string id, DateTime activation, DateTime expiry, string startString, bool active, bool isDay, string state, string timeLeft, bool isCetus, string shortString)
        {
            Id = id;
            Activation = activation;
            Expiry = expiry;
            StartString = startString;
            Active = active;
            IsDay = isDay;
            State = state;
            TimeLeft = timeLeft;
            IsCetus = isCetus;
            ShortString = shortString;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("activation")]
        public DateTime Activation { get; set; }

        [JsonPropertyName("expiry")]
        public DateTime Expiry { get; set; }

        [JsonPropertyName("startString")]
        public string StartString { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("isDay")]
        public bool IsDay { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("timeLeft")]
        public string TimeLeft { get; set; }

        [JsonPropertyName("isCetus")]
        public bool IsCetus { get; set; }

        [JsonPropertyName("shortString")]
        public string ShortString { get; set; }
    }
}