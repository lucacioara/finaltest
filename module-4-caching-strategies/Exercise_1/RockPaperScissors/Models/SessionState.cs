namespace RockPaperScissors.Models
{
    public enum SessionState
    {
        Created,
        WaitingForPlayer,
        WaitingForMoves,
        WaitingForMove,
        NextRound,
        Finished
    }
}
