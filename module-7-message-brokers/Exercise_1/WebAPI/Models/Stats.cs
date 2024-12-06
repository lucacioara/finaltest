namespace GameAPI.Models
{
    public class Stats
    {
        public string WinnerEmail { get; set; }
        public int RockCount { get; set; }
        public int PaperCount { get; set; }
        public int ScissorsCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
