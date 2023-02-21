using System.Text.Json.Serialization;

namespace WaframeDiscordBot.JSON;

public class FissuresJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("activation")]
    public DateTime Activation { get; set; }

    [JsonPropertyName("startString")]
    public string StartString { get; set; }

    [JsonPropertyName("expiry")]
    public DateTime Expiry { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("node")]
    public string Node { get; set; }

    [JsonPropertyName("missionType")]
    public string MissionType { get; set; }

    [JsonPropertyName("missionKey")]
    public string MissionKey { get; set; }

    [JsonPropertyName("enemy")]
    public string Enemy { get; set; }

    [JsonPropertyName("enemyKey")]
    public string EnemyKey { get; set; }

    [JsonPropertyName("nodeKey")]
    public string NodeKey { get; set; }

    [JsonPropertyName("tier")]
    public string Tier { get; set; }

    [JsonPropertyName("tierNum")]
    public int TierNum { get; set; }

    [JsonPropertyName("expired")]
    public bool Expired { get; set; }

    [JsonPropertyName("eta")]
    public string Eta { get; set; }

    [JsonPropertyName("isStorm")]
    public bool IsStorm { get; set; }

    [JsonPropertyName("isHard")]
    public bool IsHard { get; set; }
}