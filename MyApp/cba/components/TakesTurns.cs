using System;

namespace CBA
{
    public class TakesTurns(Entity owner) : Component(owner)
    {
        private static int _nextTurnOrder = 0;
        public int TurnOrder { get; } = _nextTurnOrder++;
        public bool IsActive { get; private set; } = true;

        public event Action<Entity>? OnTurnStart;
        public event Action<Entity>? OnTurnEnd;

        protected override void Subscribe()
        {
            // Optionally subscribe to world or other systems
        }

        public void StartTurn()
        {
            IsActive = true;
            Console.Clear();
            Console.WriteLine($"--- {Owner.GetComponent<PlayerData>()?.Name}'s Turn Start ---");
            OnTurnStart?.Invoke(Owner);
        }

        public void EndTurn()
        {
            Console.WriteLine($"--- {Owner.GetComponent<PlayerData>()?.Name}'s Turn End ---");
            OnTurnEnd?.Invoke(Owner);
            IsActive = false;
        }
    }
}
