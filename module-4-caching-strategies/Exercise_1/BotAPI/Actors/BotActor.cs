using BotAPI.Models;
using BotAPI.Services;
using Dapr.Actors.Runtime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;

namespace BotAPI.Actors
{
    public class BotActor : Actor, IBotActor
    {
        private const string BotName = "Bot";
        private string gameSessionId = string.Empty;
        private Session Session;
        private ISessionService _sessionService;
        private HubConnection Connection;
        private IConfiguration _configuration;
        public BotActor(ActorHost host, [FromServices] ISessionService sessionService, IConfiguration configuration) : base(host)
        {
            _sessionService = sessionService;
            _configuration = configuration;
        }
        public async Task JoinBotSession(string sessionId)
        {
            var bot = await StateManager.GetStateAsync<Player>(BotName);
            gameSessionId = sessionId;
            await Connection.InvokeAsync("JoinSession", sessionId);
            await _sessionService.EnterExistingGame(sessionId, bot);
        }
        public async Task<SessionState> GetSessionState()
        {
            var session = await _sessionService.GetStatus(gameSessionId);
            return session.State;
        }

        public async Task CastInVote()
        {
            var bot = await StateManager.GetStateAsync<Player>(BotName);
            bot.Choice = PickRandomChoice();
            await _sessionService.CastInVote(gameSessionId, bot);
        }
        public async Task CreateBot(Player Bot)
        {
            var url = _configuration.GetValue<string>("SESSION_URL");
            Connection = new HubConnectionBuilder()
            .WithUrl($"{url}/session-hub")
            .WithAutomaticReconnect()
            .Build();
            Connection.On<Session>("SendStatus", async (session) =>
            {
                Session = session;
                await PlayGame();
            });
            await Connection.StartAsync();
            await StateManager.SetStateAsync(BotName, Bot);
        }
        private async Task PlayGame()
        {
            var state = Session.State;
            switch (state)
            {
                case SessionState.WaitingForMove:
                    await CastInVote();
                    break;
                case SessionState.Finished:
                    await Connection.StopAsync();
                    break;
            }

        }
        private Choice PickRandomChoice()
        {
            var choices = Enum.GetValues(typeof(Choice));
            Random random = new Random();
            return (Choice)choices.GetValue(random.Next(choices.Length));
        }
    }
}
