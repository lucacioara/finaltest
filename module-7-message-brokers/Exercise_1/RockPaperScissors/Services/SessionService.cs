using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Models;

namespace RockPaperScissors.Services
{
    public interface ISessionService
    {
        Task<IActionResult> CastInVote(string sessionId, Player player);
        Task<IActionResult> CreateSession(string sessionId, Player player);
        Task<SessionStatus> GetStatus(string sessionId);
        Task<SessionResult> GetSessionResult(string sessionId);
        Task<IActionResult> InviteBot(string sessionId);
        Task<IActionResult> EnterExistingGame(string sessionId, Player player);
        Task<IActionResult> SetNumberOfRounds(string sessionId, int numberOfRounds);
        Task<IActionResult> RefreshStatus(string sessionId);
        Task<IActionResult> ForfeitGame(string sessionId, string playerId);

    }
    public class SessionService : ISessionService
    {
        private IApiService _apiService;
        private string suffix = string.Empty;

        public SessionService(IApiService apiService)
        {
            _apiService = apiService;
#if (!DEBUG)
            suffix = "/game";
#endif
        }

        public async Task<IActionResult> CastInVote(string sessionId, Player player)
        {
            return await _apiService.PostAsync<IActionResult>($"{suffix}/api/Session/send/{sessionId}", player);
        }

        public async Task<IActionResult> CreateSession(string sessionId, Player player)
        {
            return await _apiService.PostAsync<IActionResult>($"{suffix}/api/Session/create/{sessionId}", player);
        }

        public async Task<IActionResult> EnterExistingGame(string sessionId, Player player)
        {
            return await _apiService.PostAsync<IActionResult>($"{suffix}/api/Session/join/{sessionId}", player);
        }

        public async Task<IActionResult> ForfeitGame(string sessionId, string playerId)
        {
            return await _apiService.PostAsync<IActionResult>($"{suffix}/api/Session/forfeit/{sessionId}", playerId);
        }

        public async Task<SessionResult> GetSessionResult(string sessionId)
        {
            return await _apiService.GetAsync<SessionResult>($"{suffix}/api/Session/result/{sessionId}");
        }

        public async Task<SessionStatus> GetStatus(string sessionId)
        {
            return await _apiService.GetAsync<SessionStatus>($"{suffix}/api/Session/status/{sessionId}");
        }

        public async Task<IActionResult> InviteBot(string sessionId)
        {
            return await _apiService.GetAsync<IActionResult>($"{suffix}/api/Session/invite/{sessionId}");
        }

        public async Task<IActionResult> SetNumberOfRounds(string sessionId, int numberOfRounds)
        {
            return await _apiService.GetAsync<IActionResult>($"{suffix}/api/Session/round/{sessionId}/{numberOfRounds}");
        }
        public async Task<IActionResult> RefreshStatus(string sessionId)
        {
            return await _apiService.GetAsync<IActionResult>($"{suffix}/api/Session/refresh/{sessionId}/");
        }
    }
}
