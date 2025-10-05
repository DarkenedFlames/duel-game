using System;

namespace MyApp
{
    public class AuburnAmalgam : Item
    {
        public float HealthAndHealingModifier { get; init; } = 1.25f;

        public AuburnAmalgam(Player owner) : base(owner)
        {
            Name = "Auburn Amalgam";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Uncommon;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Resources["Health"].RestorationMultiplier *= HealthAndHealingModifier;
            target.Stats["MaximumHealth"].Multiplier *= HealthAndHealingModifier;
            Console.WriteLine($"{target.Name}'s maximum health and healing modifiers have been permanently increased by {HealthAndHealingModifier - 1:P}!");
        }
    }
}
