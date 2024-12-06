using Newtonsoft.Json;

namespace StatsAPI.Models
{
    public class Stats
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "winner")]
        public string WinnerEmail { get; set; }
        public int RockCount { get; set; }
        public int PaperCount { get; set; }
        public int ScissorsCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
