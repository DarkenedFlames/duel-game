using System;

namespace MyApp
{
    public class Crescent : Item
    {
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
            var hasMoonlight = Owner.ActiveEffects.Any(e => e is Moonlight);

            if (!hasMoonlight)
            {
                // First Crescent strike this turn
                Owner.ReceiveEffect(new Moonlight(Owner));

                Console.WriteLine($"{Owner.Name}'s {Name} glows with moonlight!");
                target.TakeDamage(Damage, DamageType, CanCrit, CanDodge, critBonus: 0.5f);
            }
            else
            {
                // Already used the boosted strike this turn
                target.TakeDamage(Damage, DamageType, CanCrit, CanDodge);
            }
        }

    }
}
