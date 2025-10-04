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
            MaximumStacks = 3;
            RemainingStacks = 1;
            TargetType = TargetType.Enemy;
            IsNegative = true;
            IsHidden = false;
        }

        public override void Receive()
        {
            Console.WriteLine($"{Owner.Name} is engulfed in flames!");
        }

        public override void OnStack()
        {
            if (RemainingStacks < MaximumStacks)
            {
                RemainingStacks++;
                RemainingDuration = MaximumDuration;
                Console.WriteLine($"{Owner.Name}'s Inferno stacks to {RemainingStacks}!");
            }
            else
            {
                RemainingDuration = MaximumDuration;
                Console.WriteLine($"{Owner.Name}'s Inferno is refreshed at max stacks!");
            }
        }

        public override void Tick()
        {
            int damage = Math.Max(1, (int)(Owner.MaximumHealth.Value * 0.01f));
            // Multiply by stacks
            damage *= RemainingStacks;

            Owner.TakeDamage(damage, DamageType.Magical, canCrit: false, canDodge: false);

            RemainingDuration--;
            if (RemainingDuration <= 0)
                Owner.LoseEffect(this);

            Console.WriteLine($"{Owner.Name} takes inferno damage. ({RemainingDuration} turns left)");
        }

        public override void Lose()
        {
            Console.WriteLine($"{Owner.Name} is no longer aflame.");
        }
    }
}
