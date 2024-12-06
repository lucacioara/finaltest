using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.AspNetCore.Mvc;
using StatsAPI.Models;
using StatsAPI.Services;
using System.Text.Json;

namespace StatsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventHandlerController : ControllerBase
    {
        private DatabaseService _databaseService;
        public EventHandlerController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        [HttpPost]
        public async Task<IActionResult> HandleEvent([FromBody] JsonElement eventsJson)
        {
            var documentOptions = new JsonDocumentOptions
            {
                AllowTrailingCommas = true
            };
            var serializerOptions = new JsonSerializerOptions
            {
                AllowTrailingCommas = true
            };
            using (JsonDocument document = JsonDocument.Parse(eventsJson.GetRawText(), documentOptions))
            {
                var events = System.Text.Json.JsonSerializer.Deserialize<List<EventGridEvent>>(document.RootElement.GetRawText(), serializerOptions);

                foreach (var eventGridEvent in events)
                {
                    if (eventGridEvent.EventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
                    {
                        var validationData = System.Text.Json.JsonSerializer.Deserialize<SubscriptionValidationEventData>(eventGridEvent.Data.ToString(), serializerOptions);
                        return Ok(new SubscriptionValidationResponse { ValidationResponse = validationData.ValidationCode });
                    }
                    else if (eventGridEvent.EventType == "GameCompleted")
                    {
                        var gameData = System.Text.Json.JsonSerializer.Deserialize<Stats>(eventGridEvent.Data.ToString(), serializerOptions);
                        await _databaseService.AddStats(gameData);
                        await _databaseService.CalculateStats(Period.Daily);
                        await _databaseService.CalculateStats(Period.Weekly);
                        await _databaseService.CalculateStats(Period.Monthly);
                        return Ok(gameData);
                    }
                }
            }
            return Ok();
        }
    }
}
