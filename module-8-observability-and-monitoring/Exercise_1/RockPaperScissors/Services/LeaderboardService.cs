using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Models;

namespace RockPaperScissors.Services
{
    public interface ILeaderboardService
    {
        Task<List<MoveResult>> GetMostMoves(Period period);
        Task<List<WinnerResult>> GetTopPlayers(Period period);
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
        public async Task<List<WinnerResult>> GetTopPlayers(Period period)
        {
            return await _apiService.GetAsync<List<WinnerResult>>($"{suffix}/api/Stats/TopPlayers/{period}", "Stats");
        }
        public async Task<List<MoveResult>> GetMostMoves(Period period)
        {
            return await _apiService.GetAsync<List<MoveResult>>($"{suffix}/api/Stats/TopMoves/{period}", "Stats");
        }

        public async Task<IActionResult> GenerateData(int count)
        {
            return await _apiService.PostAsync<IActionResult>($"{suffix}/api/Stats/Generate", count, "Stats");
        }
    }
}
