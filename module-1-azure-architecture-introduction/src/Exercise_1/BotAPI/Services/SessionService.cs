using BotAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BotAPI.Services
{
    public interface ISessionService
    {
        Task<IActionResult> EnterExistingGame(string gameId, Player player);
        Task<Session> GetStatus(string gameId);
        Task<IActionResult> CastInVote(string gameId, Player player);
        Task<SessionResult> GetGameResult(string gameId);
        Task<IActionResult> ReadyForNextRound(string sessionId, string playerId);

    }
    public class SessionService : ISessionService
    {
        private IApiService _apiService;

        public SessionService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> EnterExistingGame(string gameId, Player player)
        {
            return await _apiService.PostAsync<IActionResult>($"/api/Session/join/{gameId}", player);
        }
        public async Task<Session> GetStatus(string gameId)
        {
            return await _apiService.GetAsync<Session>($"/api/Session/status/{gameId}");
        }
        public async Task<IActionResult> CastInVote(string gameId, Player player)
        {
            return await _apiService.PostAsync<IActionResult>($"/api/Session/send/{gameId}", player);
        }
        public async Task<SessionResult> GetGameResult(string gameId)
        {
            return await _apiService.GetAsync<SessionResult>($"/api/Session/result/{gameId}");
        }

        public async Task<IActionResult> ReadyForNextRound(string sessionId, string playerId)
        {
            return await _apiService.GetAsync<IActionResult>($"/api/Session/ready/{sessionId}/{playerId}");
        }
    }
}
