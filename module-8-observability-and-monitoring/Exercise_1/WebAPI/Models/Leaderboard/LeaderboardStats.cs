namespace GameAPI.Models.Leaderboard
{
    public class LeaderboardStats
    {
        public string Id { get; set; }
        public string WinnerEmail { get; set; }
        public int RockCount { get; set; } = 0;
        public int PaperCount { get; set; } = 0;
        public int ScissorsCount { get; set; } = 0;
        public DateTime Timestamp { get; set; }
    }
}
