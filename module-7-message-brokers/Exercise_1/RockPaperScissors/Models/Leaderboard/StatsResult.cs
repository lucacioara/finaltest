namespace RockPaperScissors.Models.Leaderboard
{
    public class StatsResult
    {
        public string Id { get; set; }
        public string Period { get; set; }
        public List<WinnerResult> Winners { get; set; }
        public List<MoveResult> Moves { get; set; }
    }
}
