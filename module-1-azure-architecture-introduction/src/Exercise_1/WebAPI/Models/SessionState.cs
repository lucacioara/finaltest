namespace GameAPI.Models
{
    public enum SessionState
    {
        Created,
        WaitingForPlayer,
        Started,
        WaitingForMoves,
        WaitingForMove,
        WaitingForResult,
        NextRound,
        Finished
    }
}
