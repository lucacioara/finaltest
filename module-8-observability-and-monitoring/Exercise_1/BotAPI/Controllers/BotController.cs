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
        public async Task<IActionResult> JoinSession(string botId, string sessionId)
        {
            var bot = new Player { Id = botId, Name = "Bot", Choice = null, Points = 0, Email = "Bot" };
            var actorId = new ActorId(botId);
            var proxy = ActorProxy.Create<IBotActor>(actorId, nameof(BotActor));
            await proxy.CreateBot(bot);
            await proxy.JoinBotSession(sessionId);
            return Ok();
        }
        [HttpGet]
        [Route("check")]
        public async Task<HttpStatusCode> CheckAPI()
        {
            return HttpStatusCode.OK;
        }
    }
}
