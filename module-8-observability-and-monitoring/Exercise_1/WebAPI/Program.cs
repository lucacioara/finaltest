using GameAPI.Actors;
using GameAPI.Services;
using GameAPI.SignalR;
using Microsoft.AspNetCore.ResponseCompression;
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json");
    config.AddEnvironmentVariables("GAME_API_");
    config.AddCommandLine(args);
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IApiService, ApiService>();
builder.Services.AddSingleton<IBotService, BotService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<ILeaderboardService, LeaderboardService>();
builder.Services.AddSingleton(new EventGridService(config["EVENT_GRID_ENDPOINT"], config["EVENT_GRID_KEY"]));

#if DEBUG
builder.Services.AddSignalR();
#elif(!DEBUG)
var signalR = config.GetValue<string>("SIGNALR");
builder.Services.AddSignalR().AddAzureSignalR(signalR);
#endif
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<SessionActor>();
});

builder.Services.AddHttpClient("BOT", (options) =>
{
    options.BaseAddress = new Uri(config.GetValue<string>("BOTAPI"));
});
builder.Services.AddHttpClient("Stats", (options) =>
{
    options.BaseAddress = new Uri(config.GetValue<string>("STATSAPI"));
});
// Configure the HTTP request pipeline.
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.MapHub<SessionHub>("session-hub");
app.UseResponseCompression();

app.UseHttpsRedirection();
app.UseCors(policy => policy.WithOrigins(config.GetValue<string>("TRUSTEDORIGINS").Split(',')).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

app.UseAuthorization();

app.MapControllers();
app.UseRouting();
app.MapActorsHandlers();

app.Run();
