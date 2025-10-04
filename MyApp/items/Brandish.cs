using System;

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
            // Always deal damage
            target.TakeDamage(Damage, DamageType, CanCrit, CanDodge);

            // 15% chance to apply Inferno
            if (rng.NextDouble() < 0.15)
                target.ReceiveEffect(new Inferno(target));
        }
    }
}
