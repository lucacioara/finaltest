using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using RockPaperScissors;
using RockPaperScissors.Models;
using RockPaperScissors.Models.Configurations;
using RockPaperScissors.Services;
#if (!DEBUG)
using System.Net.Http.Json;
#endif
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#if (DEBUG)
builder.Services.AddHttpClient("Incognito", (options) =>
{
    options.BaseAddress = new Uri("http://localhost:5015");
});
builder.Services.AddScoped(sp =>
{
    var navMan = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl("http://localhost:5015/session-hub")
        .WithAutomaticReconnect()
        .Build();
});
builder.Services.AddSingleton<LocalConfiguration>(new LocalConfiguration());
#elif (!DEBUG)
var httpClientAPI = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
// Fetch the environment-specific GameApiUrl from the Azure Function
var config = await httpClientAPI.GetFromJsonAsync<LocalConfiguration>("api/config/gameApiUrl");
builder.Services.AddSingleton<LocalConfiguration>(new LocalConfiguration()
{
    BotApiUrl = config.BotApiUrl,
    GameApiUrl = config.GameApiUrl
});
var gameApiUrl = config?.GameApiUrl ?? "http://localhost:5015";
builder.Services.AddHttpClient("Incognito", (options) =>
{
    options.BaseAddress = new Uri(gameApiUrl);
});
builder.Services.AddScoped(sp =>
{
    var navMan = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl($"{gameApiUrl}/session-hub")
        .WithAutomaticReconnect()
        .Build();
});
#endif
builder.Services.AddHttpClient<IApiService, ApiService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddSingleton<IInviteService, InviteService>();
builder.Services.AddSingleton<IBotService, BotService>();
builder.Services.AddSingleton<Session>();
await builder.Build().RunAsync();
