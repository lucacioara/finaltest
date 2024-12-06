namespace RockPaperScissors.Models
{
    public class Session
    {
        private Player? player;
        private string? sessionId;
        private bool? isInvited;
        public Player Player
        {
            get => player ?? new Player();
            set
            {
                player = value;
                NotifyStateChanged();
            }
        }
        public string SessionId
        {
            get => sessionId ?? string.Empty;
            set
            {
                sessionId = value;
                NotifyStateChanged();
            }
        }
        public bool IsInvited
        {
            get => isInvited ?? false;
            set
            {
                isInvited = value;
                NotifyStateChanged();
            }
        }
        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
