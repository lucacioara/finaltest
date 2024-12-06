using GameAPI.Actors;
using GameAPI.Services;
using System.Reflection;
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

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<SessionActor>();
});

builder.Services.AddHttpClient("BOT", (options) =>
{
    options.BaseAddress = new Uri(config.GetValue<string>("BOTAPI"));
});
// Configure the HTTP request pipeline.
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(policy => policy.WithOrigins(config.GetValue<string>("TRUSTEDORIGINS").Split(',')).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

app.UseAuthorization();

app.MapControllers();
app.UseRouting();
app.MapActorsHandlers();
app.MapGet("/", () => DateTimeOffset.Now.ToString());
app.Run();
