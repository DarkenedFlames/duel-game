using System;

namespace MyApp
{
    public class PalePurplePotion : Item
    {
        public int ShieldAmount { get; init; } = 5;

        public PalePurplePotion(Player owner) : base(owner)
        {
            Name = "Pale Purple Potion";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Stats.IncreaseBase("Shield", ShieldAmount);
            Console.WriteLine($"{target.Name} gained {ShieldAmount} shield!");
        }
    }
}
