using Dapr.Actors;
using WebAPI.Models;

namespace GameAPI.Actors
{
    public interface ISessionActor : IActor
    {
        Task CreateSession(Player player);
        Task InviteBot();
        Task EnterExistingMatch(Player player);
        Task CastInVote(Player player);
        Task GetSessionResult();
        Task SetNumberOfRounds(int numberOfRounds);
        Task RefreshStatus();
        Task ForfeitGame(string playerID);

    }
}
