using Dapr.Actors;
using Dapr.Actors.Client;
using GameAPI.Actors;
using GameAPI.Models;
using GameAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameAPI.Controllers
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
        [HttpGet]
        [Route("status/{sessionId}")]
        public async Task<Session> GetStatus(string sessionId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            var status = await proxy.GetStatus();
            return status;
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
        [Route("result/{sessionId}")]
        public async Task<SessionResult> GetSessionResult(string sessionId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            return await proxy.GetSessionResult();
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
        [Route("ready/{sessionId}/{playerId}")]
        public async Task<IActionResult> ReadyForNextRound(string sessionId, string playerId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.ReadyForNextRound(playerId);
            return Ok();
        }
    }
}
