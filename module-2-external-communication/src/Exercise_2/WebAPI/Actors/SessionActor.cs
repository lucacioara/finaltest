using Dapr.Actors.Runtime;
using GameAPI.Models;
using GameAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using WebAPI.Models;

namespace GameAPI.Actors
{
    public class SessionActor : Actor, ISessionActor
    {
        private const string SessionName = "Session";
        private SessionResult RoundResult = SessionResult.Unfinished;
        private HubConnection Connection;
        private IEmailService _emailService;
        private IConfiguration _configuration;
        private bool IsForfeit = false;

        public SessionActor(ActorHost host, [FromServices] IEmailService emailService, IConfiguration configuration) : base(host)
        {
            _emailService = emailService;
            _configuration = configuration;
        }
        public async Task CastInVote(Player player)
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            if (session.Player1.Id == player.Id)
                session.Player1.Choice = player.Choice;
            if (session.Player2.Id == player.Id)
                session.Player2.Choice = player.Choice;
            if (session.Player1.Choice != null && session.Player2.Choice != null)
                await GetSessionResult();
            else if (session.Player1.Choice != null ^ session.Player2.Choice != null)
                await SetState(SessionState.WaitingForMove);
            await StateManager.SetStateAsync(SessionName, session);
        }

        public async Task CreateSession(Player player)
        {
            var url = _configuration.GetValue<string>("HOST");
            Connection = new HubConnectionBuilder()
            .WithUrl($"{url}/session-hub")
            .WithAutomaticReconnect()
            .Build();
            await Connection.StartAsync();
            await Connection.InvokeAsync("JoinSession", Id);
            await StateManager.SetStateAsync(SessionName, new Session { Player1 = player });
            await SetState(SessionState.Created);
        }

        public async Task EnterExistingMatch(Player player)
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            session.Player2 = player;
            await SetState(SessionState.WaitingForMoves);
            await StateManager.SetStateAsync(SessionName, session);
        }
        public async Task InviteBot()
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            await SetState(SessionState.WaitingForPlayer);
            await StateManager.SetStateAsync(SessionName, session);
        }

        public async Task GetSessionResult()
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            await GetRoundWinner(session);
            await GetSessionWinner(session);
            await StateManager.SetStateAsync(SessionName, session);

        }
        private async Task GetRoundWinner(Session session)
        {
            var choice1 = session.Player1.Choice.Value;
            var choice2 = session.Player2.Choice.Value;
            if (choice1 == choice2)
            {
                RoundResult = SessionResult.Tie;
            }
            else if ((choice1 == Choice.Rock && choice2 == Choice.Scissors) || (choice1 == Choice.Scissors && choice2 == Choice.Paper) || (choice1 == Choice.Paper && choice2 == Choice.Rock))
            {
                RoundResult = SessionResult.Player1;

            }
            else if ((choice2 == Choice.Rock && choice1 == Choice.Scissors) || (choice2 == Choice.Scissors && choice1 == Choice.Paper) || (choice2 == Choice.Paper && choice1 == Choice.Rock))
            {
                RoundResult = SessionResult.Player2;
            }
            AddPointsToWinner(session);
            await Connection.InvokeAsync("SendRoundResult", RoundResult, Id);

        }
        private void AddPointsToWinner(Session session)
        {
            switch (RoundResult)
            {
                case SessionResult.Player1:
                    session.Player1.Points++;
                    break;
                case SessionResult.Player2:
                    session.Player2.Points++;
                    break;
            }
        }
        private async Task GetSessionWinner(Session session)
        {
            if (session.Player1.Points == session.MaxRounds / 2 + 1)
            {
                session.Result = SessionResult.Player1;
            }
            else if (session.Player2.Points == session.MaxRounds / 2 + 1)
            {
                session.Result = SessionResult.Player2;
            }
            else
            {
                await SetState(SessionState.NextRound);
                await Task.Delay(5000);
                await IncrementRound(session);
                return;
            }
            await StateManager.SetStateAsync(SessionName, session);
            await SetState(SessionState.Finished);
            SendResultEmail(session);
            await Connection.StopAsync();
        }
        private async Task IncrementRound(Session session)
        {
            if (session.State == SessionState.NextRound)
            {
                switch (RoundResult)
                {
                    case SessionResult.Player1:
                    case SessionResult.Player2:
                        session.CurrentRound++;
                        break;
                    case SessionResult.Tie:
                        break;
                }
                session.Player1.Choice = null;
                session.Player2.Choice = null;
                await SetState(SessionState.WaitingForMoves);
                RoundResult = SessionResult.Unfinished;
            }
        }
        public async Task SetNumberOfRounds(int numberOfRounds)
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            session.MaxRounds = numberOfRounds;
            await StateManager.SetStateAsync(SessionName, session);
        }
        private async Task SetState(SessionState state)
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            session.State = state;
            await StateManager.SetStateAsync(SessionName, session);
            await Connection.InvokeAsync("SendStatus", session, Id);
        }
        public async Task RefreshStatus()
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            await Connection.InvokeAsync("SendStatus", session, Id);
        }
        private void SendResultEmail(Session session)
        {
            _emailService.SendSessionResult(session);
        }
        public async Task ForfeitGame(string playerID)
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            if (playerID == session.Player1.Id)
            {
                session.Result = SessionResult.Player2;
                RoundResult = session.Result;
            }
            else if (playerID == session.Player2.Id)
            {
                session.Result = SessionResult.Player1;
                RoundResult = session.Result;
            }
            IsForfeit = true;
            await SetState(SessionState.Finished);
            await StateManager.SetStateAsync(SessionName, session);
        }
    }
}
