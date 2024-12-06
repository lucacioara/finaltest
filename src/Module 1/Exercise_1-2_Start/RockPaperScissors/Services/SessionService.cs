using RockPaperScissors.Models;

namespace RockPaperScissors.Services
{
    public interface ISessionService
    {
        Task<int> CastInVote(string sessionId, string playerId, Choice choice);
        Task<int> CreateSession(string playerId, string sessionId);
        Task<SessionStatus> GetStatus(string sessionId);
        Task<SessionResult> GetSessionResult(string sessionId);
        Task<int> InviteBot(string sessionId);
        Task<int> EnterExistingGame(string sessionId, string playerId);
        Task<int> ReadyForNextRound(string sessionId, string playerId);
        Task<int> SetNumberOfRounds(string sessionId, int numberOfRounds);
    }
    public class SessionService : ISessionService
    {
        private IApiService _apiService;

        public SessionService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<int> CastInVote(string sessionId, string playerId, Choice choice)
        {
            return await _apiService.GetAsync<int>($"/api/Session/send/{sessionId}/{playerId}/{choice}");

        }

        public async Task<int> CreateSession(string playerId, string sessionId)
        {
            return await _apiService.GetAsync<int>($"/api/Session/create/{playerId}/{sessionId}");
        }

        public async Task<int> EnterExistingGame(string sessionId, string playerId)
        {
            return await _apiService.GetAsync<int>($"/api/Session/join/{sessionId}/{playerId}");

        }

        public async Task<SessionResult> GetSessionResult(string sessionId)
        {
            return await _apiService.GetAsync<SessionResult>($"/api/Session/result/{sessionId}");

        }

        public async Task<SessionStatus> GetStatus(string sessionId)
        {
            return await _apiService.GetAsync<SessionStatus>($"/api/Session/status/{sessionId}");

        }

        public async Task<int> InviteBot(string sessionId)
        {
            return await _apiService.GetAsync<int>($"/api/Session/invite/{sessionId}");

        }
        public async Task<int> ReadyForNextRound(string sessionId, string playerId)
        {
            return await _apiService.GetAsync<int>($"/api/Session/ready/{sessionId}/{playerId}");
        }
        public async Task<int> SetNumberOfRounds(string sessionId, int numberOfRounds)
        {
            return await _apiService.GetAsync<int>($"/api/Session/round/{sessionId}/{numberOfRounds}");
        }

    }
}
