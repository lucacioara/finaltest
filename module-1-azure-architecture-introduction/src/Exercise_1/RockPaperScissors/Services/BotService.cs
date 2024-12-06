using System.Net;
using System.Net.Http.Json;

namespace RockPaperScissors.Services
{
    public interface IBotService
    {
        Task<HttpStatusCode> CheckBotAPI();

    }
    public class BotService : IBotService
    {
        private readonly HttpClient client;

        private IConfiguration configuration;
        public BotService(HttpClient client, IConfiguration configuration)
        {
            this.client = client;
            this.configuration = configuration;
        }


        public async Task<HttpStatusCode> CheckBotAPI()
        {
            try
            {
                var url = configuration["BOTAPI_URL"];
                var result = await client.GetAsync($"{url}/api/Bot/check");
                result.EnsureSuccessStatusCode();
                var data = await result.Content.ReadFromJsonAsync<HttpStatusCode>();
                return data;
            }
            catch (Exception ex)
            {
                return HttpStatusCode.ServiceUnavailable;
            }
        }
    }
}
