using System;

namespace MyApp
{
    public class PupilPorridge : Item
    {
        public int CriticalAmount { get; init; } = 5;

        public PupilPorridge(Player owner) : base(owner)
        {
            Name = "Pupil Porridge";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Stats.IncreaseBase("Critical", CriticalAmount);
            Console.WriteLine($"{target.Name} gained {CriticalAmount} critical!");
        }
    }
}
