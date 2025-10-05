using System;

namespace MyApp
{
    public class EarthyElixir : Item
    {
        public float ArmorAndShieldModifier { get; init; } = 1.25f;

        public EarthyElixir(Player owner) : base(owner)
        {
            Name = "Earthy Elixir";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Uncommon;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Stats["Armor"].Multiplier *= ArmorAndShieldModifier;
            target.Stats["Shield"].Multiplier *= ArmorAndShieldModifier;
            Console.WriteLine($"{target.Name}'s shield and armor have been permanently increased by {ArmorAndShieldModifier - 1:P}!");
        }
    }
}
