using Microsoft.AspNetCore.SignalR;
using WebAPI.Models;

namespace GameAPI.SignalR
{
    public class SessionHub : Hub
    {
        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }
        public async Task SendStatus(Session session, string sessionId)
        {
            await Clients.Group(sessionId).SendAsync("SendStatus", session);
        }
        public async Task SendRoundResult(SessionResult result, string sessionId)
        {
            await Clients.Group(sessionId).SendAsync("SendRoundResult", result);
        }
    }
}
