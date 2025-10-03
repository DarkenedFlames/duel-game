using System;
using System.Linq;

namespace MyApp
{
    public class Brandish : Item
    {
        private static readonly Random rng = new Random();

        public Brandish(Player owner) : base(owner)
        {
            Name = "Brandish";
            Type = ItemType.Weapon;
            Rarity = ItemRarity.Common;
            DamageType = DamageType.Physical;
            Damage = 17;
            StaminaCost = 16;
            CanCrit = true;
            CanDodge = true;
        }

        public override void Use(Player target)
        {
            // Always apply normal damage
            target.TakeDamage(Damage, DamageType, CanCrit, CanDodge);

            // Roll chance for Inferno (15%)
            if (rng.NextDouble() < 0.15)
            {
                var existing = target.ActiveEffects.FirstOrDefault(e => e is Inferno);

                if (existing != null)
                {
                    // Refresh duration
                    existing.RemainingDuration = existing.MaximumDuration;
                    Console.WriteLine($"{target.Name}'s Inferno is refreshed!");
                }
                else
                {
                    // Apply new Inferno
                    target.ReceiveEffect(new Inferno(target));
                }
            }
        }
    }
}
