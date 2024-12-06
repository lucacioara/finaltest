using Dapr.Actors;
using GameAPI.Models;

namespace GameAPI.Actors
{
    public interface ISessionActor : IActor
    {
        Task CreateSession(Player player);
        Task InviteBot();
        Task EnterExistingMatch(Player player);
        Task<Session> GetStatus();
        Task CastInVote(Player player);
        Task<SessionResult> GetSessionResult();
        Task ReadyForNextRound(string playerID);
        Task SetNumberOfRounds(int numberOfRounds);
        Task ForfeitGame(string playerID);
    }
}
