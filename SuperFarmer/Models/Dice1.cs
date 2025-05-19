namespace SuperFarmer.Models
{
    public class Dice1 : Dice
    {
        public Dice1()
        {
            Sides = new List<Animal>
            {
                Animal.Rabbit, Animal.Rabbit, Animal.Rabbit,
                Animal.Rabbit, Animal.Rabbit, Animal.Rabbit,
                Animal.Sheep, Animal.Sheep, Animal.Sheep,
                Animal.Pig,
                Animal.Cow,
                Animal.Wolf
            };
        }
    }

}