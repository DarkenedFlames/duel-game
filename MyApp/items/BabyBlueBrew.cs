using System;

namespace MyApp
{
    public class BabyBlueBrew : Item
    {
        public int StaminaAmount { get; init; } = 25;

        public BabyBlueBrew(Player owner) : base(owner)
        {
            Name = "Baby Blue Brew";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 0;
        }

        public override void Use(Player target)
        {
            target.Resources.Change("Stamina", StaminaAmount);
            Console.WriteLine($"{target.Name} restored {StaminaAmount} stamina!");
        }
    }
}
