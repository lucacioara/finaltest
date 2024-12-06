using BotAPI.Models;

namespace BotAPI.Services
{
    public interface ISessionService
    {
        Task<int> EnterExistingGame(string gameId, string playerId);
        Task<Session> GetStatus(string gameId);
        Task<int> CastInVote(string gameId, string playerId, Choice choice);
        Task<SessionResult> GetGameResult(string gameId);
        Task<int> ReadyForNextRound(string sessionId, string playerId);

    }
    public class SessionService : ISessionService
    {
        private IApiService _apiService;

        public SessionService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<int> EnterExistingGame(string gameId, string playerId)
        {
            return await _apiService.GetAsync<int>($"/api/Session/join/{gameId}/{playerId}");

        }
        public async Task<Session> GetStatus(string gameId)
        {
            return await _apiService.GetAsync<Session>($"/api/Session/status/{gameId}");

        }
        public async Task<int> CastInVote(string gameId, string playerId, Choice choice)
        {
            return await _apiService.GetAsync<int>($"/api/Session/send/{gameId}/{playerId}/{choice}");
        }
        public async Task<SessionResult> GetGameResult(string gameId)
        {
            return await _apiService.GetAsync<SessionResult>($"/api/Session/result/{gameId}");

        }

        public async Task<int> ReadyForNextRound(string sessionId, string playerId)
        {
            return await _apiService.GetAsync<int>($"/api/Session/ready/{sessionId}/{playerId}");

        }
    }
}
