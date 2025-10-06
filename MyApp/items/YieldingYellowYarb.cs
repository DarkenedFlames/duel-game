using System;

namespace MyApp
{
    public class YieldingYellowYarb : Item
    {
        public int ArmorAmount { get; init; } = 5;

        public YieldingYellowYarb(Player owner) : base(owner)
        {
            Name = "Yielding Yellow Yarb";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Stats.IncreaseBase("Armor", ArmorAmount);
            Console.WriteLine($"{target.Name} gained {ArmorAmount} armor!");
        }
    }
}
