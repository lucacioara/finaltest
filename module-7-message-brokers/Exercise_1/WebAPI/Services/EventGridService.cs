using Azure;
using Azure.Messaging.EventGrid;
using GameAPI.Models.Leaderboard;

namespace GameAPI.Services
{
    public class EventGridService
    {
        private EventGridPublisherClient _eventGridClient;
        public EventGridService(string endpoint, string key)
        {
            var credential = new AzureKeyCredential(key);
            _eventGridClient = new EventGridPublisherClient(new Uri(endpoint), credential);
        }
        public async Task PublishGameEvent(LeaderboardStats stats)
        {
            var eventGridEvent = new EventGridEvent
                (
                subject: "GameAPI.GameCompleted",
                eventType: "GameCompleted",
                dataVersion: "1.0",
                data: stats
                );
            await _eventGridClient.SendEventAsync(eventGridEvent);
        }
    }
}
