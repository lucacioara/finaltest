using RockPaperScissors.Services;

namespace RockPaperScissors.ViewModels
{
    public class SessionViewModel
    {
        private ISessionService _sessionService;

        public SessionViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

    }
}
