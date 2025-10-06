using System;

namespace MyApp
{
    public class OozingOrangeOil : Item
    {
        public int MaximumHealthAmount { get; init; } = 5;

        public OozingOrangeOil(Player owner) : base(owner)
        {
            Name = "Oozing Orange Oil";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Stats.IncreaseBase("MaximumHealth", MaximumHealthAmount);
            Console.WriteLine($"{target.Name} gained {MaximumHealthAmount} maximum health!");
        }
    }
}
