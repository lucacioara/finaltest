using Newtonsoft.Json;

namespace StatsAPI.Models
{
    public class StatsResult
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "period")]
        public string Period { get; set; }
        public List<WinnerResult> Winners { get; set; }
        public List<MoveResult> Moves { get; set; }
    }
}