using System;

namespace MyApp
{
    public class Moonlight : Effect
    {
        public Moonlight(Player owner) : base(owner)
        {
            Name = "Moonlight";
            MaximumDuration = 1;
            RemainingDuration = 1;
            MaximumStacks = 1;
            RemainingStacks = 1;
            TargetType = TargetType.Self;
            IsNegative = true;
        }

        public override void Tick()
        {

            if (RemainingDuration > 0)
            {
                RemainingDuration--;
                Console.WriteLine($"{Owner.Name} is illuminated by moonlight. ({RemainingDuration} turns remaining)");
            }
            else
            {
                Owner.LoseEffect(this);
                Console.WriteLine($"{Owner.Name} is no longer illuminated by moonlight.");
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
