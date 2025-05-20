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
        
        public bool TryExchange(
            Animal target,
            Dictionary<Animal, (Animal fromAnimal, int cost)> exchangeRates,
            Dictionary<Animal, int> bank)
        {
            if (!exchangeRates.TryGetValue(target, out var rule))
                return false;

            var fromAnimal = rule.fromAnimal;
            var cost = rule.cost;

            if (!Animals.ContainsKey(fromAnimal) || Animals[fromAnimal] < cost)
                return false;

            if (!bank.ContainsKey(target) || bank[target] < 1)
                return false;
            
            Animals[fromAnimal] -= cost;
            if (Animals[fromAnimal] == 0)
                Animals.Remove(fromAnimal);

            if (!bank.ContainsKey(fromAnimal))
                bank[fromAnimal] = 0;
            bank[fromAnimal] += cost;

            bank[target] -= 1;

            if (!Animals.ContainsKey(target))
                Animals[target] = 0;
            Animals[target] += 1;

            return true;
        }

        
    } 
}

