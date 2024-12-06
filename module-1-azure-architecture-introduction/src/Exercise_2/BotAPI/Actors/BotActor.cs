using BotAPI.Models;
using BotAPI.Services;
using Dapr.Actors.Runtime;
using Microsoft.AspNetCore.Mvc;

namespace BotAPI.Actors
{
    public class BotActor : Actor, IBotActor
    {
        private const string BotName = "Bot";
        private string gameSessionId = string.Empty;
        private ISessionService _sessionService;
        public BotActor(ActorHost host, [FromServices] ISessionService sessionService) : base(host)
        {
            _sessionService = sessionService;
        }
        public async Task JoinBotSession(string sessionId)
        {
            var bot = await StateManager.GetStateAsync<Player>(BotName);
            gameSessionId = sessionId;
            await _sessionService.EnterExistingGame(sessionId, bot);
            Task.Run(() => PlayGame());

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
        private async Task GetResult()
        {
            await _sessionService.GetGameResult(gameSessionId);
        }
        private async Task ReadyForNextRound()
        {
            var bot = await StateManager.GetStateAsync<Player>(BotName);
            await _sessionService.ReadyForNextRound(gameSessionId, bot.Id);
        }
        public async Task CreateBot(Player Bot)
        {
            await StateManager.SetStateAsync(BotName, Bot);

        }
        private async Task PlayGame()
        {
            SessionState state;
            do
            {
                state = await GetSessionState();
                if (state == SessionState.WaitingForMove)
                    await CastInVote();
                else if (state == SessionState.WaitingForResult)
                    await GetResult();
                else if (state == SessionState.NextRound)
                {
                    await ReadyForNextRound();
                }
                await Task.Delay(500);
            } while (state != SessionState.Finished);
        }
        private Choice PickRandomChoice()
        {
            var choices = Enum.GetValues(typeof(Choice));
            Random random = new Random();
            return (Choice)choices.GetValue(random.Next(choices.Length));
        }
    }
}
