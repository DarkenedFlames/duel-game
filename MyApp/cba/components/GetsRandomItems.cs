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
            if (player == null) return;

            // --- Step 1: Create the first item ---
            var firstItem = ItemFactory.CreateRandomItem(player);
            Printer.PrintEntityAdded(firstItem);

            // --- Step 2: Check for bonus item based on Luck ---
            var stats = player.GetComponent<StatsComponent>();
            float luck = stats?.Get("Luck") ?? 0f;
            float chance = luck / (luck + 100f);

            if (Random.Shared.NextDouble() < chance)
            {
                var secondItem = ItemFactory.CreateRandomItem(player);
                Printer.PrintEntityAdded(secondItem);
            }
        }
    }
}
