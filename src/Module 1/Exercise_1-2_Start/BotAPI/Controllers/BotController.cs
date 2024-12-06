using BotAPI.Actors;
using BotAPI.Models;
using BotAPI.Services;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BotAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BotController : ControllerBase
    {
        private ISessionService _sessionService;

        public BotController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        [Route("join/{botId}/{sessionId}")]
        public async Task<int> JoinSession(string botId, string sessionId)
        {
            var bot = new Player { Id = botId, Name = "Bot", Choice = null, Points = 0, Ready = false };
            Console.WriteLine(botId);
            var actorId = new ActorId(botId);
            Console.WriteLine(actorId);
            var proxy = ActorProxy.Create<IBotActor>(actorId, nameof(BotActor));
            await proxy.CreateBot(bot);
            await proxy.JoinBotSession(sessionId);
            return 0;
        }
        [HttpGet]
        [Route("check")]
        public async Task<HttpStatusCode> CheckAPI()
        {
            return HttpStatusCode.OK;
        }
    }
}
