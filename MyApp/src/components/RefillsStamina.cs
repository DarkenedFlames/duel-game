using System;

namespace CBA
{
    public class RefillsStamina(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            var resources = Owner.GetComponent<ResourcesComponent>();
            if (resources != null)
            {
                var turns = Owner.GetComponent<TakesTurns>();
                if (turns != null)
                {
                    turns.OnTurnStart += RefillStamina;
                }
            }
        }

        private void RefillStamina(Entity owner)
        {
            var resources = Owner.GetComponent<ResourcesComponent>();
            resources?.Refill("Stamina");
        }
        


    }

}