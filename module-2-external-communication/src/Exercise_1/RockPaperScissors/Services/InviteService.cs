using RockPaperScissors.Models;

namespace RockPaperScissors.Services
{
    interface IInviteService
    {
        public Task<short> SendInvite(EmailData data);
    }
    public class InviteService : IInviteService
    {
        private IApiService _apiService;

        public InviteService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<short> SendInvite(EmailData data)
        {
            return await _apiService.PostAsync<short>("/api/Invite/SendInvite", data);
        }
    }
}
