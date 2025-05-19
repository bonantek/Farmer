namespace SuperFarmer.Models
{
    public class Game
    {

        public Guid Id { get; set; } = Guid.NewGuid();

        public List<Player> Players { get; set; } = new();

        public int CurrentPlayerIndex { get; set; } = 0;

        public bool IsOver { get; set; } = false;

        public Player? Winner { get; set; }
        
        public int CurrentRound { get; set; } = 1;

        public Player CurrentPlayer => Players[CurrentPlayerIndex];
        
        public (Animal, Animal)? LastRoll { get; set; }
        
        public bool DiceRolledThisTurn { get; set; } = false;



        public Game(List<Player> players)
        {
            Players = players;
            CurrentPlayerIndex = 0;
        }
        
        public void NextTurn()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
            if (CurrentPlayerIndex == 0)
            {
                CurrentRound++;
            }
            
            DiceRolledThisTurn = false;
            LastRoll = null;
        }
        
        public void RollDice()
        {
            var d1 = new Dice1();
            var d2 = new Dice2();
            LastRoll = (d1.Roll(), d2.Roll());
            
            DiceRolledThisTurn = true;
        }
        
        public void Breed()
        {
            if (LastRoll == null) return;

            var currentPlayer = CurrentPlayer;
            var (roll1, roll2) = LastRoll.Value;
            
            var rolled = new Dictionary<Animal, int>();

            foreach (var animal in new[] { roll1, roll2 })
            {
                if (!rolled.ContainsKey(animal))
                    rolled[animal] = 1;
                else
                    rolled[animal]++;
            }

            var breedable = new[] { Animal.Rabbit, Animal.Sheep, Animal.Pig };

            foreach (var type in breedable)
            {
                if (!rolled.ContainsKey(type)) continue;
                
                if (!currentPlayer.Animals.ContainsKey(type)) continue;

                int owned = currentPlayer.Animals[type]; 
                int fromDice = rolled.ContainsKey(type) ? rolled[type] : 0;

                int total = owned + fromDice;
                int pairs = total / 2;

                if (pairs > 0)
                {
                    currentPlayer.Animals[type] += pairs;
                }
            }
        }

        


        
    }    
}

