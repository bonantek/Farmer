namespace SuperFarmer.Models
{
    public class Dice2 : Dice
    {
        public Dice2()
        {
            Sides = new List<Animal>
            {
                Animal.Sheep, Animal.Sheep, Animal.Sheep,
                Animal.Sheep, Animal.Sheep, Animal.Sheep,
                Animal.Pig, Animal.Pig,
                Animal.Rabbit, Animal.Rabbit,
                Animal.Horse,
                Animal.Fox
            };
        }
    }

}
