namespace GameAPI.Services
{
    public interface IBotService
    {
        Task<int> CreateBot(string sessionId);
    }
    public class BotService : IBotService
    {
        private IApiService _apiService;

        public BotService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<int> CreateBot(string sessionId)
        {
            var id = Guid.NewGuid().ToString();
            var result = await _apiService.GetAsync<int>($"/api/Bot/join/{id}/{sessionId}");
            return result;
        }
    }
}
