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
        
        public Dictionary<Animal, (Animal fromAnimal, int cost)> ExchangeRates { get; set; }
        // public bool HasExchangedThisTurn { get; set; } = false;

        public Game(List<Player> players)
        {
            Players = players;
            CurrentPlayerIndex = 0;
            
            ExchangeRates = new Dictionary<Animal, (Animal, int)>
            {
                { Animal.Sheep,    (Animal.Rabbit, 6) },
                { Animal.Pig,      (Animal.Sheep, 2) },
                { Animal.Cow,      (Animal.Pig, 3) },
                { Animal.Horse,    (Animal.Cow, 2) },
                { Animal.SmallDog, (Animal.Sheep, 1) },
                { Animal.BigDog,   (Animal.Cow, 1) }
            };
        }
        
        public void NextTurn()
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
            if (CurrentPlayerIndex == 0)
            {
                CurrentRound++;
            }
            
            DiceRolledThisTurn = false;
            // HasExchangedThisTurn = false;
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
        
        public void AnimalEating()
        {
            if (LastRoll == null) return;

            var currentPlayer = CurrentPlayer;
            var (roll1, roll2) = LastRoll.Value;

            bool foxRolled = false;
            bool wolfRolled = false;

            if (roll1 == Animal.Fox) foxRolled = true;
            if (roll2 == Animal.Fox) foxRolled = true;

            if (roll1 == Animal.Wolf) wolfRolled = true;
            if (roll2 == Animal.Wolf) wolfRolled = true;

            if (foxRolled)
            {
                if (currentPlayer.Animals.ContainsKey(Animal.SmallDog) && currentPlayer.Animals[Animal.SmallDog] > 0)
                {
                    currentPlayer.Animals[Animal.SmallDog] -= 1;

                    if (currentPlayer.Animals[Animal.SmallDog] == 0)
                        currentPlayer.Animals.Remove(Animal.SmallDog);
                }
                else
                {
                    if (currentPlayer.Animals.ContainsKey(Animal.Rabbit) && currentPlayer.Animals[Animal.Rabbit] > 1)
                    {
                        currentPlayer.Animals[Animal.Rabbit] = 1;
                    }
                }
            }

            if (wolfRolled)
            {
                if (currentPlayer.Animals.ContainsKey(Animal.BigDog) && currentPlayer.Animals[Animal.BigDog] > 0)
                {
                    currentPlayer.Animals[Animal.BigDog] -= 1;

                    if (currentPlayer.Animals[Animal.BigDog] == 0)
                        currentPlayer.Animals.Remove(Animal.BigDog);
                }
                else
                {
                    var protectedAnimals = new[] { Animal.Rabbit, Animal.Horse, Animal.SmallDog };
                    
                    // blad -- nie da sie usuwac jesli sie aktualenie chodzi w foreach ;) 
                    foreach (var animal in currentPlayer.Animals.Keys.ToList())
                    {
                        if (!protectedAnimals.Contains(animal))
                        {
                            currentPlayer.Animals.Remove(animal);
                        }
                    }
                }
            }
        }

        
        
        
        
    }    
}

