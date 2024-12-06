using GameAPI.Models.Leaderboard;
using Microsoft.AspNetCore.Mvc;

namespace GameAPI.Services
{
    public interface ILeaderboardService
    {
        Task<IActionResult> SendSessionResult(LeaderboardStats stats);
    }

    public class LeaderboardService : ILeaderboardService
    {
        private IApiService _apiService;
        public LeaderboardService(IApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<IActionResult> SendSessionResult(LeaderboardStats stats)
        {
            return await _apiService.PostAsync<IActionResult>("/api/Stats/SendResult", stats, "Stats");
        }
    }
}
