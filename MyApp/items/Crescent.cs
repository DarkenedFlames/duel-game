using System;
using System.Linq;

namespace MyApp
{
    public class Crescent : Item
    {
        private const float CritBonus = 0.5f; // 50% bonus

        public Crescent(Player owner) : base(owner)
        {
            Name = "Crescent";
            Type = ItemType.Weapon;
            Rarity = ItemRarity.Common;
            DamageType = DamageType.Physical;
            Damage = 18;
            StaminaCost = 15;
            CanCrit = true;
            CanDodge = true;
        }

        public override void Use(Player target)
        {
            // Check if owner already has Moonlight
            bool hasMoonlight = Owner.ActiveEffects.Any(e => e is Moonlight);

            float bonus = hasMoonlight ? 0f : CritBonus;

            // Deal damage with optional bonus crit
            target.TakeDamage(Damage, DamageType, CanCrit, CanDodge, critBonus: bonus);

            // If first hit this turn, apply Moonlight marker
            if (!hasMoonlight)
                Owner.ReceiveEffect(new Moonlight(Owner));
        }
    }
}
