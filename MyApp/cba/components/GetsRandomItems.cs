using System;

namespace CBA
{
    public class GetsRandomItems(Entity owner) : Component(owner)
    {
        protected override void Subscribe()
        {
            // Subscribe to the player's turn start event
            var takesTurns = Owner.GetComponent<TakesTurns>();
            if (takesTurns != null)
            {
                takesTurns.OnTurnStart += GiveRandomItem;
            }
        }

        private void GiveRandomItem(Entity player)
        {
            // Player is the owner of this component
            var newItem = ItemFactory.CreateRandomItem(player);
            // Notify printer (optional)
            Printer.PrintEntityAdded(newItem); // Fix this, probably needs to be an event
        }
    }
}
