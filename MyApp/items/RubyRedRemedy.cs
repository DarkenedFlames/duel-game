using System;

namespace MyApp
{
    public class RubyRedRemedy : Item
    {
        public int HealAmount { get; init; } = 25;

        public RubyRedRemedy(Player owner) : base(owner)
        {
            Name = "Ruby Red Remedy";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Health.Change(HealAmount);
            Console.WriteLine($"{target.Name} healed {HealAmount} health!");
        }
    }
}
