using Polly;
using Polly.CircuitBreaker;
using RockPaperScissors.Models.Configurations;
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
        private LocalConfiguration _localConfiguration;
        private readonly AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreakerPolicy;
        public bool IsCircuitOpen { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;

        private IConfiguration configuration;
        public BotService(HttpClient client, IConfiguration configuration, LocalConfiguration localConfiguration)
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
            _localConfiguration = localConfiguration;
            this.client = client;
            this.configuration = configuration;
        }


        public async Task<HttpStatusCode> CheckBotAPI()
        {
            try
            {
                var response = await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
#if (DEBUG)
                    var url = configuration["BOTAPI_URL"];
                    var result = await client.GetAsync($"{url}/api/Bot/check");
                    result.EnsureSuccessStatusCode();
                    return result;

#elif (!DEBUG)
                    var url = _localConfiguration?.BotApiUrl ?? "http://localhost:5080";
                    var result = await client.GetAsync($"{url}/api/Bot/check");
                    result.EnsureSuccessStatusCode();
                    return result;
#endif
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
