using Dapr.Actors.Runtime;
using GameAPI.Models;

namespace GameAPI.Actors
{
    public class SessionActor : Actor, ISessionActor
    {
        public SessionActor(ActorHost host) : base(host)
        {
        }
        private const string SessionName = "Session";
        private SessionResult Result = SessionResult.Unfinished;
        public async Task CastInVote(Player player)
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            if (session.Player1.Id == player.Id)
                session.Player1.Choice = player.Choice;
            if (session.Player2.Id == player.Id)
                session.Player2.Choice = player.Choice;
            if (session.Player1.Choice != null && session.Player2.Choice != null)
                session.State = SessionState.WaitingForResult;
            else if (session.Player1.Choice != null ^ session.Player2.Choice != null)
                session.State = SessionState.WaitingForMove;
            await StateManager.SetStateAsync(SessionName, session);
        }
        public async Task CreateSession(Player player)
        {
            await StateManager.SetStateAsync(SessionName, new Session { Player1 = player, State = SessionState.Created });
        }
        public async Task EnterExistingMatch(Player player)
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            session.Player2 = player;
            session.State = SessionState.WaitingForMoves;
            await StateManager.SetStateAsync(SessionName, session);
        }
        public async Task<Session> GetStatus()
        {
            return await StateManager.GetStateAsync<Session>(SessionName);
        }
        public async Task InviteBot()
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            session.State = SessionState.WaitingForPlayer;
            await StateManager.SetStateAsync(SessionName, session);
        }
        public async Task<SessionResult> GetSessionResult()
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            if (Result == SessionResult.Unfinished)
                await GetRoundWinner(session);
            var sessionResult = await GetSessionWinner(session);
            if (sessionResult == SessionResult.Unfinished)
                session.State = SessionState.NextRound;
            else
            {
                session.State = SessionState.Finished;
                session.Result = sessionResult;
            }
            await StateManager.SetStateAsync(SessionName, session);
            return Result;
        }
        private async Task<SessionResult> GetRoundWinner(Session session)
        {
            var choice1 = session.Player1.Choice.Value;
            var choice2 = session.Player2.Choice.Value;
            if (choice1 == choice2)
            {
                Result = SessionResult.Tie;
            }
            else if ((choice1 == Choice.Rock && choice2 == Choice.Scissors) || (choice1 == Choice.Scissors && choice2 == Choice.Paper) || (choice1 == Choice.Paper && choice2 == Choice.Rock))
            {
                Result = SessionResult.Player1;

            }
            else if ((choice2 == Choice.Rock && choice1 == Choice.Scissors) || (choice2 == Choice.Scissors && choice1 == Choice.Paper) || (choice2 == Choice.Paper && choice1 == Choice.Rock))
            {
                Result = SessionResult.Player2;

            }
            AddPointsToWinner(session);
            return Result;
        }
        private void AddPointsToWinner(Session session)
        {
            switch (Result)
            {
                case SessionResult.Player1:
                    session.Player1.Points++;
                    break;
                case SessionResult.Player2:
                    session.Player2.Points++;
                    break;
            }
        }
        private async Task IncrementRound(Session session)
        {
            if (session.State == SessionState.NextRound)
            {
                switch (Result)
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
                session.Player1.Ready = false;
                session.Player2.Ready = false;
                session.State = SessionState.WaitingForMoves;
                Result = SessionResult.Unfinished;
                await StateManager.SetStateAsync(SessionName, session);
            }
        }
        private async Task<SessionResult> GetSessionWinner(Session session)
        {
            if (session.Player1.Points == session.MaxRounds / 2 + 1)
                return SessionResult.Player1;
            else if (session.Player2.Points == session.MaxRounds / 2 + 1)
                return SessionResult.Player2;
            else
            {
                return SessionResult.Unfinished;
            }
        }
        public async Task ReadyForNextRound(string playerID)
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            if (session.Player1.Id == playerID)
                session.Player1.Ready = true;
            if (session.Player2.Id == playerID)
                session.Player2.Ready = true;
            if (session.Player1.Ready && session.Player2.Ready)
            {
                await IncrementRound(session);
            }
        }
        public async Task SetNumberOfRounds(int numberOfRounds)
        {
            var session = await StateManager.GetStateAsync<Session>(SessionName);
            session.MaxRounds = numberOfRounds;
            await StateManager.SetStateAsync(SessionName, session);
        }
    }
}
