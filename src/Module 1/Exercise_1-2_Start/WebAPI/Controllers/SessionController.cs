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

        [HttpGet]
        [Route("create/{playerId}/{sessionId}")]
        public async Task<int> CreateSession(string playerId, string sessionId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));

            await proxy.CreateSession(playerId);
            return 0;
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
        [HttpGet]
        [Route("send/{sessionId}/{playerId}/{choice}")]
        public async Task<int> CastInVote(string sessionId, string playerId, Choice choice)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.CastInVote(playerId, choice);
            return 0;
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
        public async Task<int> SetNumberOfRounds(string sessionId, int numberOfRounds)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.SetNumberOfRounds(numberOfRounds);
            return 0;
        }
        [HttpGet]
        [Route("invite/{sessionId}")]
        public async Task<int> InviteBot(string sessionId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            var bot = await _botService.CreateBot(sessionId);
            return 0;
        }
        [HttpGet]
        [Route("join/{sessionId}/{playerId}")]
        public async Task<int> EnterExistingMatch(string sessionId, string playerId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.EnterExistingMatch(playerId);
            return 0;

        }
        [HttpGet]
        [Route("ready/{sessionId}/{playerId}")]
        public async Task<int> ReadyForNextRound(string sessionId, string playerId)
        {
            var actorId = new ActorId(sessionId);
            var proxy = ActorProxy.Create<ISessionActor>(actorId, nameof(SessionActor));
            await proxy.ReadyForNextRound(playerId);
            return 0;
        }

    }
}
