using Newtonsoft.Json;

namespace WaframeDiscordBot.JSON;

public class EventJson
{
    public class InterimStep 
    {
        public InterimStep(int goal, Reward reward)
        {
            Goal = goal;
            Reward = reward;
        }

        [JsonProperty("goal")]
        public int Goal { get; set; }

        [JsonProperty("reward")]
        public Reward Reward { get; set; }
    }

    public class Reward
    {
        public Reward(List<string> items, List<object> countedItems, int credits, string asString, string itemString, string thumbnail, int color)
        {
            Items = items;
            CountedItems = countedItems;
            Credits = credits;
            AsString = asString;
            ItemString = itemString;
            Thumbnail = thumbnail;
            Color = color;
        }

        [JsonProperty("items")]
        public List<string> Items { get; set; }

        [JsonProperty("countedItems")]
        public List<object> CountedItems { get; set; }

        [JsonProperty("credits")]
        public int Credits { get; set; }

        [JsonProperty("asString")]
        public string AsString { get; set; }

        [JsonProperty("itemString")]
        public string ItemString { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("color")]
        public int Color { get; set; }
    }

    public class Reward2
    {
        public Reward2(List<string> items, List<object> countedItems, int credits, string asString, string itemString, string thumbnail, int color)
        {
            Items = items;
            CountedItems = countedItems;
            Credits = credits;
            AsString = asString;
            ItemString = itemString;
            Thumbnail = thumbnail;
            Color = color;
        }

        [JsonProperty("items")]
        public List<string> Items { get; set; }

        [JsonProperty("countedItems")]
        public List<object> CountedItems { get; set; }

        [JsonProperty("credits")]
        public int Credits { get; set; }

        [JsonProperty("asString")]
        public string AsString { get; set; }

        [JsonProperty("itemString")]
        public string ItemString { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("color")]
        public int Color { get; set; }
    }

    public class Root
    {
        public Root(string id, DateTime activation, string startString, DateTime expiry, bool active, int maximumScore, int currentScore, object smallInterval, object largeInterval, string description, string tooltip, string node, List<object> concurrentNodes, string scoreLocTag, List<Reward> rewards, bool expired, int health, List<InterimStep> interimSteps, List<object> progressSteps, bool isPersonal, bool isCommunity, List<object> regionDrops, List<object> archwingDrops, string asString, List<object> completionBonuses, string scoreVar, DateTime altExpiry, DateTime altActivation)
        {
            Id = id;
            Activation = activation;
            StartString = startString;
            Expiry = expiry;
            Active = active;
            MaximumScore = maximumScore;
            CurrentScore = currentScore;
            SmallInterval = smallInterval;
            LargeInterval = largeInterval;
            Description = description;
            Tooltip = tooltip;
            Node = node;
            ConcurrentNodes = concurrentNodes;
            ScoreLocTag = scoreLocTag;
            Rewards = rewards;
            Expired = expired;
            Health = health;
            InterimSteps = interimSteps;
            ProgressSteps = progressSteps;
            IsPersonal = isPersonal;
            IsCommunity = isCommunity;
            RegionDrops = regionDrops;
            ArchwingDrops = archwingDrops;
            AsString = asString;
            CompletionBonuses = completionBonuses;
            ScoreVar = scoreVar;
            AltExpiry = altExpiry;
            AltActivation = altActivation;
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

        [JsonProperty("maximumScore")]
        public int MaximumScore { get; set; }

        [JsonProperty("currentScore")]
        public int CurrentScore { get; set; }

        [JsonProperty("smallInterval")]
        public object SmallInterval { get; set; }

        [JsonProperty("largeInterval")]
        public object LargeInterval { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tooltip")]
        public string Tooltip { get; set; }

        [JsonProperty("node")]
        public string Node { get; set; }

        [JsonProperty("concurrentNodes")]
        public List<object> ConcurrentNodes { get; set; }

        [JsonProperty("scoreLocTag")]
        public string ScoreLocTag { get; set; }

        [JsonProperty("rewards")]
        public List<Reward> Rewards { get; set; }

        [JsonProperty("expired")]
        public bool Expired { get; set; }

        [JsonProperty("health")]
        public int Health { get; set; }

        [JsonProperty("interimSteps")]
        public List<InterimStep> InterimSteps { get; set; }

        [JsonProperty("progressSteps")]
        public List<object> ProgressSteps { get; set; }

        [JsonProperty("isPersonal")]
        public bool IsPersonal { get; set; }

        [JsonProperty("isCommunity")]
        public bool IsCommunity { get; set; }

        [JsonProperty("regionDrops")]
        public List<object> RegionDrops { get; set; }

        [JsonProperty("archwingDrops")]
        public List<object> ArchwingDrops { get; set; }

        [JsonProperty("asString")]
        public string AsString { get; set; }

        [JsonProperty("completionBonuses")]
        public List<object> CompletionBonuses { get; set; }

        [JsonProperty("scoreVar")]
        public string ScoreVar { get; set; }

        [JsonProperty("altExpiry")]
        public DateTime AltExpiry { get; set; }

        [JsonProperty("altActivation")]
        public DateTime AltActivation { get; set; }
    }
}