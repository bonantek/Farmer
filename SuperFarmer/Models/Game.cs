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
        
        public Dictionary<Animal, int> Bank { get; set; }
        
        public Dictionary<Animal, int> AddedToHerd { get; set; } = new();
        
        public string GetAnimalImagePath(Animal animal)
        {
            return $"/images/{animal.ToString().ToLower()}.png";
        }
        

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
            
            Bank = new Dictionary<Animal, int>
            {
                { Animal.Rabbit, 60 },
                { Animal.Sheep, 24 },
                { Animal.Pig, 20 },
                { Animal.Cow, 12 },
                { Animal.Horse, 6 },
                { Animal.SmallDog, 4 },
                { Animal.BigDog, 2 }
            };

        }
        
        public static Game InitializeWithPlayers(int playerCount)
        {
            var temp = new Game(new());

            var players = new List<Player>();
            for (int i = 1; i <= playerCount; i++)
            {
                var player = new Player(i);
                if (temp.Bank[Animal.Rabbit] > 0)
                {
                    player.Animals[Animal.Rabbit] = 1;
                    temp.Bank[Animal.Rabbit] -= 1;
                }
                players.Add(player);
            }

            return new Game(players) { Bank = temp.Bank };
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
            AddedToHerd.Clear();
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
            AddedToHerd.Clear(); 

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
                int fromDice = rolled.ContainsKey(type) ? rolled[type] : 0;
                int owned = currentPlayer.Animals.ContainsKey(type) ? currentPlayer.Animals[type] : 0;

                int total = fromDice + owned;
                int pairs = total / 2;

                if (pairs > 0)
                {
                    int available = Bank.ContainsKey(type) ? Bank[type] : 0;
                    int toGive = Math.Min(pairs, available);

                    if (toGive > 0)
                    {
                        if (!currentPlayer.Animals.ContainsKey(type))
                            currentPlayer.Animals[type] = 0;

                        currentPlayer.Animals[type] += toGive;
                        Bank[type] -= toGive;
                        
                        if (!AddedToHerd.ContainsKey(type))
                            AddedToHerd[type] = 0;

                        AddedToHerd[type] += toGive;
                    }
                }
            }
        }

        public void AnimalEating()
        {
            if (LastRoll == null) return;

            var currentPlayer = CurrentPlayer;
            var (roll1, roll2) = LastRoll.Value;

            bool foxRolled = roll1 == Animal.Fox || roll2 == Animal.Fox;
            bool wolfRolled = roll1 == Animal.Wolf || roll2 == Animal.Wolf;
            
            if (foxRolled)
            {
                if (currentPlayer.Animals.ContainsKey(Animal.SmallDog) && currentPlayer.Animals[Animal.SmallDog] > 0)
                {
                    currentPlayer.Animals[Animal.SmallDog] -= 1;
                    if (currentPlayer.Animals[Animal.SmallDog] == 0)
                        currentPlayer.Animals.Remove(Animal.SmallDog);

                    Bank[Animal.SmallDog] = Bank.GetValueOrDefault(Animal.SmallDog) + 1;
                }
                else if (currentPlayer.Animals.ContainsKey(Animal.Rabbit))
                {
                    int lost = currentPlayer.Animals[Animal.Rabbit] - 1;
                    if (lost > 0)
                    {
                        currentPlayer.Animals[Animal.Rabbit] = 1;
                        Bank[Animal.Rabbit] = Bank.GetValueOrDefault(Animal.Rabbit) + lost;
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

                    Bank[Animal.BigDog] = Bank.GetValueOrDefault(Animal.BigDog) + 1;
                }
                else
                {
                    var protectedAnimals = new[] { Animal.Rabbit, Animal.Horse, Animal.SmallDog };

                    var animalsToRemove = currentPlayer.Animals.Keys
                        .Where(a => !protectedAnimals.Contains(a))
                        .ToList();

                    foreach (var animal in animalsToRemove)
                    {
                        int amount = currentPlayer.Animals[animal];
                        currentPlayer.Animals.Remove(animal);
                        Bank[animal] = Bank.GetValueOrDefault(animal) + amount;
                    }
                }
            }
        }

        
        public int? CheckVictory()
        {
            var required = new[] { Animal.Rabbit, Animal.Sheep, Animal.Pig, Animal.Cow, Animal.Horse };

            for (int i = 0; i < Players.Count; i++)
            {
                var player = Players[i];
                bool hasAll = required.All(animal =>
                    player.Animals.ContainsKey(animal) && player.Animals[animal] > 0);

                if (hasAll)
                {
                    Winner = player;
                    return i; 
                }
            }

            return null;
        }


        public List<(int targetPlayerIndex, Animal targetAnimal, int targetAmount, Animal offeredAnimal, int
                offeredAmount)>
            GetPossibleTradesWithOtherPlayers(Player currentPlayer, Animal offeredAnimal)
        {
            var trades = new List<(int, Animal, int, Animal, int)>();

            if (!ExchangeRates.Values.Any(e => e.fromAnimal == offeredAnimal))
                return trades;

            foreach (var kvp in ExchangeRates)
            {
                var targetAnimal = kvp.Key;
                var (fromAnimal, cost) = kvp.Value;

                if (fromAnimal != offeredAnimal)
                    continue;

                int howManySetsPlayerCanOffer = currentPlayer.Animals.TryGetValue(offeredAnimal, out var ownedAmount)
                    ? ownedAmount / cost
                    : 0;

                if (howManySetsPlayerCanOffer == 0)
                    continue;

                for (int i = 0; i < Players.Count; i++)
                {
                    var otherPlayer = Players[i];
                    if (otherPlayer == currentPlayer) continue;

                    if (otherPlayer.Animals.TryGetValue(targetAnimal, out var hasTarget))
                    {
                        int maxTradable = Math.Min(howManySetsPlayerCanOffer, hasTarget);
                        if (maxTradable > 0)
                        {
                            trades.Add((i, targetAnimal, maxTradable, offeredAnimal, cost * maxTradable));
                        }
                    }
                }
            }

            return trades;
        }
    }    
}

