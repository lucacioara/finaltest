namespace BotAPI.Models
{
    public class Player
    {
        public string Id { get; set; }
        public Choice? Choice { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Points { get; set; }
    }
}
