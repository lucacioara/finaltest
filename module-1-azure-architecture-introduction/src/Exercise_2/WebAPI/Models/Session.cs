namespace GameAPI.Models
{
    public class Session
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int MaxRounds { get; set; } = 3;
        public int CurrentRound { get; set; } = 1;
        public SessionState State { get; set; }
        public SessionResult Result { get; set; }
    }
}
