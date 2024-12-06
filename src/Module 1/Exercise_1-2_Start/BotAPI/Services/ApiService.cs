using System.Text;
using System.Text.Json;

namespace BotAPI.Services
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string apiFunction);
        Task<T> PostAsync<T>(string apiFunction, object data);

    }
    public class ApiService : IApiService
    {

        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private JsonSerializerOptions _jsonWriter;
        private ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
            _jsonWriter = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string apiFunction)
        {
            T tResult = default(T);
            try
            {
                _logger.LogInformation($"This logs apifunction {apiFunction}");
                var http = _httpClientFactory.CreateClient("SESSION");
                _logger.LogInformation($"This logs http base address and apifunction {http.BaseAddress} , {apiFunction}");
                string queryRequest = apiFunction;
                using (var response = await http.GetAsync(queryRequest))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var responseData = JsonSerializer.Deserialize<T>(result, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        if (responseData != null)
                        {
                            tResult = responseData;
                        }

                    }
                }
                _logger.LogInformation($"This was called after using");
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"This is a message from catch exception: {ex.Message}");
                throw ex;
            }
            return tResult;
        }
        public async Task<T> PostAsync<T>(string apiFunction, object data)
        {
            T tResult = default(T);
            var http = _httpClientFactory.CreateClient("SESSION");
            var body = JsonSerializer.Serialize(data);

            string queryRequest = apiFunction;

            try
            {
                using (var response = await http.PostAsync(queryRequest, new StringContent(body, Encoding.UTF8, "application/json")))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var responseData = JsonSerializer.Deserialize<T>(result, _jsonWriter);
                        if (responseData != null)
                        {
                            tResult = responseData;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tResult;
        }
    }
}
