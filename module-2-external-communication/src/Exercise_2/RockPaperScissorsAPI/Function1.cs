using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace RockPaperScissorsAPI
{
    public static class Function1
    {
        [Function("GetGameApiUrl")]
        public static async Task<IActionResult> Run(
         [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "config/gameApiUrl")] HttpRequest req,
         ILogger log)
        {
            // Fetch environment variable from Azure
            string gameApiUrl = Environment.GetEnvironmentVariable("GAMEAPI_URL") ?? "http://localhost:5015";
            string apimUrl = Environment.GetEnvironmentVariable("APIM_URL") ?? "http://localhost:5080";
            return new OkObjectResult(new { GameApiUrl = gameApiUrl, APIMUrl = apimUrl });
        }
    }
}
