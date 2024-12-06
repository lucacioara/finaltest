namespace BotAPI.Models
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
