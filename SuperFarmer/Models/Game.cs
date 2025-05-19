namespace SuperFarmer.Models
{
    public class Game
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public List<Player> Players { get; set; } = new();

        public int CurrentPlayerIndex { get; set; } = 0;

        public bool IsOver { get; set; } = false;

        public Player? Winner { get; set; }

        public Player CurrentPlayer => Players[CurrentPlayerIndex];

        public Game(List<Player> players)
        {
            Players = players;
            CurrentPlayerIndex = 0;
        }
        
    }    
}

