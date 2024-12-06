using StatsAPI.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json");
    config.AddEnvironmentVariables("STATS_API_");
    config.AddCommandLine(args);
});
builder.Services.AddSingleton(new DatabaseService(config["DB_CONNECTION_STRING"], config));
builder.Services.AddSingleton<IMockGenerationService, MockGenerationService>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors(policy => policy.WithOrigins(config.GetValue<string>("TRUSTEDORIGINS").Split(',')).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

app.UseAuthorization();

app.MapControllers();

app.Run();
