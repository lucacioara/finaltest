using Microsoft.AspNetCore.Mvc;

namespace GameAPI.Services
{
    public interface IBotService
    {
        Task CreateBot(string sessionId);
    }
    public class BotService : IBotService
    {
        private IApiService _apiService;

        public BotService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task CreateBot(string sessionId)
        {
            var id = Guid.NewGuid().ToString();
            await _apiService.GetAsync<IActionResult>($"/api/Bot/join/{id}/{sessionId}");
        }
    }
}
