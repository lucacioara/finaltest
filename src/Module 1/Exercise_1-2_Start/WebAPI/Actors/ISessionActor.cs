using Dapr.Actors;
using WebAPI.Models;

namespace GameAPI.Actors
{
    public interface ISessionActor : IActor
    {
        Task CreateSession(string playerID);
        Task InviteBot();
        Task EnterExistingMatch(string playerID);
        Task<Session> GetStatus();
        Task CastInVote(string playerID, Choice choice);
        Task<SessionResult> GetSessionResult();
        Task ReadyForNextRound(string playerID);
        Task SetNumberOfRounds(int numberOfRounds);
    }
}
