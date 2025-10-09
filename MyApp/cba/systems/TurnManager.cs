using System;
using System.Linq;
using System.Collections.Generic;

namespace CBA
{
    public class TurnManager
    {
        private int _turnIndex = 0;


        public void StartGameLoop()
        {
            var turnEntities = World.Instance.GetEntitiesWith<TakesTurns>().ToList();

            if (turnEntities.Count == 0)
            {
                Console.WriteLine("No entities have TakesTurns component.");
                return;
            }

            while (turnEntities.Count > 1)
            {
                // Remove deleted or dead entities
                turnEntities = World.Instance.GetEntitiesWith<TakesTurns>().ToList();
                if (turnEntities.Count == 0) break;

                // Sort by their unique TurnOrder (monotonic ID)
                var ordered = turnEntities
                    .Select(e => e.GetComponent<TakesTurns>())
                    .OrderBy(t => t!.TurnOrder)
                    .ToList();

                // Clamp turn index
                if (_turnIndex >= ordered.Count)
                    _turnIndex = 0;

                var current = ordered[_turnIndex];
                if (current == null || !current.IsActive)
                {
                    _turnIndex++;
                    continue;
                }

                // START TURN
                current.StartTurn();

                // Here you could await user input, do actions, etc.
                // For now we simulate the player's menu and end turn manually.
                Console.WriteLine($"[{current.Owner.GetComponent<PlayerData>()?.Name}] is taking their turn...");
                Console.ReadKey(true);

                // END TURN
                current.EndTurn();

                // Advance turn
                _turnIndex++;
            }

            Console.WriteLine("\nGame Over.");
        }
    }
}
