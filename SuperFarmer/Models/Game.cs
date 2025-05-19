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
            
            var (roll1, roll2) = LastRoll.Value;
            var currentPlayer = CurrentPlayer;

            void AddAnimal(Animal animal)
            {
                if (!currentPlayer.Animals.ContainsKey(animal))
                    currentPlayer.Animals[animal] = 1;
                else
                    currentPlayer.Animals[animal] += 1;
            }

            AddAnimal(roll1);
            AddAnimal(roll2);

            DiceRolledThisTurn = true;
        }


        
    }    
}

