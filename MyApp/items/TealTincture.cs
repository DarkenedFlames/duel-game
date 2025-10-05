using System;

namespace MyApp
{
    public class TealTincture : Item
    {
        public float StaminaAndStimmingModifier { get; init; } = 1.25f;

        public TealTincture(Player owner) : base(owner)
        {
            Name = "Teal Tincture";
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Uncommon;
            StaminaCost = 15;
        }

        public override void Use(Player target)
        {
            target.Resources["Stamina"].RestorationMultiplier *= StaminaAndStimmingModifier;
            target.Stats["MaximumStamina"].Multiplier *= StaminaAndStimmingModifier;
            Console.WriteLine($"{target.Name}'s maximum stamina and stimming modifiers have been permanently increased by {StaminaAndStimmingModifier - 1:P}!");
        }
    }
}
