using BotAPI.Actors;
using BotAPI.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json");
    config.AddEnvironmentVariables("BOT_API_");
    config.AddCommandLine(args);
});
// Add services to the container.
builder.Services.AddHttpClient<IApiService, ApiService>();
builder.Services.AddSingleton<ISessionService, SessionService>();
builder.Services.AddControllers();
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<BotActor>();
});
builder.Services.AddHttpClient("SESSION", (options) =>
{
    options.BaseAddress = new Uri(config.GetValue<string>("SESSION_URL"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors(policy => policy.WithOrigins(config.GetValue<string>("TRUSTEDORIGINS").Split(',')).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

app.UseAuthorization();

app.MapControllers();
app.UseRouting();
app.MapActorsHandlers();

app.Run();
