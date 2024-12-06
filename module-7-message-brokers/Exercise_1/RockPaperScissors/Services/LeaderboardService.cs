using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Models;
using RockPaperScissors.Models.Leaderboard;

namespace RockPaperScissors.Services
{
    public interface ILeaderboardService
    {
        Task<StatsResult> GetStats(Period period);
        Task<IActionResult> GenerateData(int count);
    }

    public class LeaderboardService : ILeaderboardService
    {
        private IApiService _apiService;
        private string suffix = string.Empty;

        public LeaderboardService(IApiService apiService)
        {
            _apiService = apiService;
#if (!DEBUG)
suffix="/stats";
#endif
        }
        public async Task<StatsResult> GetStats(Period period)
        {
            return await _apiService.GetAsync<StatsResult>($"{suffix}/api/Stats/GetStats/{period}", "Stats");
        }

        public async Task<IActionResult> GenerateData(int count)
        {
            return await _apiService.PostAsync<IActionResult>($"{suffix}/api/Stats/Generate", count, "Stats");
        }
    }
}
