using System.Text;
using System.Text.Json;

namespace RockPaperScissors.Services
{
    public interface IApiService
    {
        Task<T> GetAsync<T>(string apiFunction, string client = "Game");
        Task<T> PostAsync<T>(string apiFunction, object data, string client = "Game");

    }
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private JsonSerializerOptions _jsonWriter;

        public ApiService(HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
            _jsonWriter = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<T> GetAsync<T>(string apiFunction, string client = "Game")
        {
            T tResult = default(T);
            var http = _httpClientFactory.CreateClient(client);

            string queryRequest = apiFunction;

            try
            {
                using (var response = await http.GetAsync(queryRequest))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrEmpty(result))
                            return default;
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
        public async Task<T> PostAsync<T>(string apiFunction, object data, string client = "Game")
        {
            T tResult = default(T);
            var http = _httpClientFactory.CreateClient(client);
            var body = JsonSerializer.Serialize(data);

            string queryRequest = apiFunction;

            try
            {
                using (var response = await http.PostAsync(queryRequest, new StringContent(body, Encoding.UTF8, "application/json")))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrEmpty(result))
                            return default;
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
