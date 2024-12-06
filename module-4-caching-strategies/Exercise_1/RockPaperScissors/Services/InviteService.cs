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
        private string suffix = string.Empty;

        public InviteService(IApiService apiService)
        {
            _apiService = apiService;
#if (!DEBUG)
suffix="/game";
#endif
        }

        public async Task<short> SendInvite(EmailData data)
        {
            return await _apiService.PostAsync<short>($"{suffix}/api/Invite/SendInvite", data);
        }
    }
}
