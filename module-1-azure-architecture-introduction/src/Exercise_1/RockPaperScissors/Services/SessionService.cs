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
        Task<IActionResult> ReadyForNextRound(string sessionId, string playerId);
        Task<IActionResult> SetNumberOfRounds(string sessionId, int numberOfRounds);
        Task<IActionResult> ForfeitGame(string sessionId, string playerId);
    }
    public class SessionService : ISessionService
    {
        private IApiService _apiService;

        public SessionService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> CastInVote(string sessionId, Player player)
        {
            return await _apiService.PostAsync<IActionResult>($"/api/Session/send/{sessionId}", player);
        }

        public async Task<IActionResult> CreateSession(string sessionId, Player player)
        {
            return await _apiService.PostAsync<IActionResult>($"/api/Session/create/{sessionId}", player);
        }

        public async Task<IActionResult> EnterExistingGame(string sessionId, Player player)
        {
            return await _apiService.PostAsync<IActionResult>($"/api/Session/join/{sessionId}", player);
        }

        public async Task<IActionResult> ForfeitGame(string sessionId, string playerId)
        {
            return await _apiService.PostAsync<IActionResult>($"/api/Session/forfeit/{sessionId}", playerId);
        }

        public async Task<SessionResult> GetSessionResult(string sessionId)
        {
            return await _apiService.GetAsync<SessionResult>($"/api/Session/result/{sessionId}");
        }

        public async Task<SessionStatus> GetStatus(string sessionId)
        {
            return await _apiService.GetAsync<SessionStatus>($"/api/Session/status/{sessionId}");
        }

        public async Task<IActionResult> InviteBot(string sessionId)
        {
            return await _apiService.GetAsync<IActionResult>($"/api/Session/invite/{sessionId}");
        }

        public async Task<IActionResult> ReadyForNextRound(string sessionId, string playerId)
        {
            return await _apiService.GetAsync<IActionResult>($"/api/Session/ready/{sessionId}/{playerId}");
        }

        public async Task<IActionResult> SetNumberOfRounds(string sessionId, int numberOfRounds)
        {
            return await _apiService.GetAsync<IActionResult>($"/api/Session/round/{sessionId}/{numberOfRounds}");
        }

    }
}
