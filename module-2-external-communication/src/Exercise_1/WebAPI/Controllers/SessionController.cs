using Dapr.Actors;
using Dapr.Actors.Client;
using GameAPI.Actors;
using GameAPI.Services;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {

        private IBotService _botService;
        public SessionController(IBotService botService)
        {
            _botService = botService;
        }

        [HttpPost]
        [Route("create/{sessionId}")]
        public async Task<IActionResult> CreateSession(string sessionId, [FromBody] Player player)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.CreateSession(player);
            return Ok();
        }
        [HttpPost]
        [Route("send/{sessionId}")]
        public async Task<IActionResult> CastInVote(string sessionId, [FromBody] Player player)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.CastInVote(player);
            return Ok();
        }
        [HttpGet]
        [Route("round/{sessionId}/{numberOfRounds}")]
        public async Task<IActionResult> SetNumberOfRounds(string sessionId, int numberOfRounds)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.SetNumberOfRounds(numberOfRounds);
            return Ok();
        }
        [HttpGet]
        [Route("invite/{sessionId}")]
        public async Task<IActionResult> InviteBot(string sessionId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await _botService.CreateBot(sessionId);
            return Ok();
        }
        [HttpPost]
        [Route("join/{sessionId}")]
        public async Task<IActionResult> EnterExistingMatch(string sessionId, Player player)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.EnterExistingMatch(player);
            return Ok();
        }
        [HttpGet]
        [Route("refresh/{sessionId}")]
        public async Task<IActionResult> RefreshStatus(string sessionId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.RefreshStatus();
            return Ok();
        }
        [HttpPost]
        [Route("forfeit/{sessionId}")]
        public async Task<IActionResult> ForfeitGame(string sessionId, [FromBody] string playerId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.ForfeitGame(playerId);
            return Ok();
        }
    }
}
