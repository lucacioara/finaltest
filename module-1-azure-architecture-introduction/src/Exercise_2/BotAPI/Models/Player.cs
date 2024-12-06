namespace BotAPI.Models
{
    public class Player
    {
        public string Id { get; set; }
        public Choice? Choice { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public bool Ready { get; set; }
    }
}
