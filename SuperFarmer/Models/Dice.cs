namespace SuperFarmer.Models
{
    public abstract class Dice
    {
        protected List<Animal> Sides = new();
        protected Random _random = new();

        public Animal Roll()
        {
            int index = _random.Next(Sides.Count);
            return Sides[index];
        }
    }

}
