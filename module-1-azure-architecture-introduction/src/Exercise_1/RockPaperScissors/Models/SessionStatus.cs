namespace RockPaperScissors.Models
{
    public class SessionStatus
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int MaxRounds { get; set; }
        public int CurrentRound { get; set; }
        public SessionState State { get; set; }
        public SessionResult Result { get; set; }
    }
}
