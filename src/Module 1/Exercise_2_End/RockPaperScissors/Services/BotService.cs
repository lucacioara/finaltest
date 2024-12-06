using Polly;
using Polly.CircuitBreaker;
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
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
        public bool IsCircuitOpen { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;

        private IConfiguration configuration;
        public BotService(HttpClient client, IConfiguration configuration)
        {
            _circuitBreakerPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    IsCircuitOpen = true;
                    ErrorMessage = "The service is temporarily unavailable. Please try again later.";
                },
                onReset: () =>
                {
                    IsCircuitOpen = false;
                    ErrorMessage = string.Empty;
                },
                onHalfOpen: () =>
                {
                    ErrorMessage = "Circuit is half-open; testing the next call.";
                }
            );
            this.client = client;
            this.configuration = configuration;
        }


        public async Task<HttpStatusCode> CheckBotAPI()
        {
            try
            {
                var response = await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    var url = configuration["BOTAPI_URL"];
                    var result = await client.GetAsync($"{url}/api/Bot/check");
                    result.EnsureSuccessStatusCode();
                    return result;
                });
                var data = await response.Content.ReadFromJsonAsync<HttpStatusCode>();
                return data;
            }
            catch (Exception ex)
            {
                return HttpStatusCode.ServiceUnavailable;
            }
        }
    }
}
