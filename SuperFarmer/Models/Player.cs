namespace SuperFarmer.Models
{
    public class Player
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int Index { get; set; }

        public Dictionary<Animal, int> Animals { get; set; }

        public Player(int index)
        {
            Index = index;
            Animals = new Dictionary<Animal, int>();
        }
        
    } 
}

