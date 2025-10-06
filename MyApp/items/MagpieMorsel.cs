using System;

namespace MyApp
{
    public class MagpieMorsel : Item
    {
        public int LuckAmount { get; init; } = 5;

        public MagpieMorsel(Player owner) : base(owner)
        {
            Name = "Magpie Morsel";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Stats.IncreaseBase("Luck", LuckAmount);
            Console.WriteLine($"{target.Name} gained {LuckAmount} luck!");
        }
    }
}
