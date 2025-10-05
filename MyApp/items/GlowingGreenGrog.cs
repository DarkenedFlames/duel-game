using System;

namespace MyApp
{
    public class GlowingGreenGrog : Item
    {
        public int MaximumStaminaAmount { get; init; } = 5;

        public GlowingGreenGrog(Player owner) : base(owner)
        {
            Name = "Glowing Green Grog";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Stats["MaximumStamina"].Base += MaximumStaminaAmount;
            Console.WriteLine($"{target.Name} gained {MaximumStaminaAmount} maximum stamina!");
        }
    }
}
