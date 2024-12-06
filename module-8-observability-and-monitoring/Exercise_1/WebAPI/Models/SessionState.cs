namespace GameAPI.Models
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
