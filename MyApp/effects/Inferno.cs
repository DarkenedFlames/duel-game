using System;

namespace MyApp
{
    public class Inferno : Effect
    {
        public Inferno(Player owner) : base(owner)
        {
            Name = "Inferno";
            MaximumDuration = 3;
            RemainingDuration = 3;
            MaximumStacks = 1;
            RemainingStacks = 1;
            TargetType = TargetType.Enemy;
            IsNegative = true;
        }

        public override void Tick()
        {
            if (RemainingDuration > 0)
            {
                // 1% of max health per tick (at least 1 damage)
                int damage = Math.Max(1, (int)(Owner.MaximumHealth.Value * 0.01f));

                Owner.TakeDamage(damage, DamageType.Magical, canCrit: false, canDodge: false);

                RemainingDuration--;
                Console.WriteLine($"{Owner.Name} is engulfed by the {Name}. ({RemainingDuration} turns remaining)");
            }
            else
            {
                Owner.LoseEffect(this);
                Console.WriteLine($"{Owner.Name} is no longer engulfed by the {Name}.");
            }
        }

        public override void Receive()
        {
            // Implementation for when the effect is applied to a target
        }

        public override void Lose()
        {
            // Implementation for when the effect is removed from a target
        }
    }
}
