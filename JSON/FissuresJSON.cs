using Newtonsoft.Json;

namespace WaframeDiscordBot.JSON;

public class FissuresJson
{
    public class Root
    {
        public Root(string id, DateTime activation, string startString, DateTime expiry, bool active, string node, string missionType, string missionKey, string enemy, string enemyKey, string nodeKey, string tier, int tierNum, bool expired, string eta, bool isStorm, bool isHard)
        {
            Id = id;
            Activation = activation;
            StartString = startString;
            Expiry = expiry;
            Active = active;
            Node = node;
            MissionType = missionType;
            MissionKey = missionKey;
            Enemy = enemy;
            EnemyKey = enemyKey;
            NodeKey = nodeKey;
            Tier = tier;
            TierNum = tierNum;
            Expired = expired;
            Eta = eta;
            IsStorm = isStorm;
            IsHard = isHard;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("activation")]
        public DateTime Activation { get; set; }

        [JsonProperty("startString")]
        public string StartString { get; set; }

        [JsonProperty("expiry")]
        public DateTime Expiry { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("node")]
        public string Node { get; set; }

        [JsonProperty("missionType")]
        public string MissionType { get; set; }

        [JsonProperty("missionKey")]
        public string MissionKey { get; set; }

        [JsonProperty("enemy")]
        public string Enemy { get; set; }

        [JsonProperty("enemyKey")]
        public string EnemyKey { get; set; }

        [JsonProperty("nodeKey")]
        public string NodeKey { get; set; }

        [JsonProperty("tier")]
        public string Tier { get; set; }

        [JsonProperty("tierNum")]
        public int TierNum { get; set; }

        [JsonProperty("expired")]
        public bool Expired { get; set; }

        [JsonProperty("eta")]
        public string Eta { get; set; }

        [JsonProperty("isStorm")]
        public bool IsStorm { get; set; }

        [JsonProperty("isHard")]
        public bool IsHard { get; set; }
    }
}