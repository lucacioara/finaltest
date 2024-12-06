using BotAPI.Models;
using Dapr.Actors;

namespace BotAPI.Actors
{
    public interface IBotActor : IActor
    {
        Task CreateBot(Player Bot);
        Task JoinBotSession(string sessionId);
        Task<SessionState> GetSessionState();
        Task CastInVote();

    }
}
