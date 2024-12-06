using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RockPaperScissors;
using RockPaperScissors.Models;
using RockPaperScissors.Services;
using System.Net.Http.Json;
using RockPaperScissors.Models.Configurations;
using RockPaperScissors.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#if (DEBUG)
builder.Services.AddHttpClient("Incognito", (options) =>
{
    options.BaseAddress = new Uri("http://localhost:5015");
});
#elif (!DEBUG)
var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

// Fetch the environment-specific GameApiUrl from the Azure Function
var config = await httpClient.GetFromJsonAsync<LocalConfiguration>("api/config/gameApiUrl");
var gameApiUrl = config?.GameApiUrl ?? "http://localhost:5015";
builder.Services.AddHttpClient("Incognito", (options) =>
{
    options.BaseAddress = new Uri(gameApiUrl);
});
Console.WriteLine(gameApiUrl);
#endif


builder.Services.AddHttpClient<IApiService, ApiService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IBotService, BotService>();
builder.Services.AddSingleton<IBotService, BotService>();
builder.Services.AddSingleton<Session>();


await builder.Build().RunAsync();
