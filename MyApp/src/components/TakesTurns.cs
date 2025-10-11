namespace CBA
{
    public class TakesTurns(Entity owner) : Component(owner)
    {
        private static int _nextTurnOrder = 0;
        public int TurnOrder { get; } = _nextTurnOrder++;
        public bool IsActive { get; private set; } = true;

        public event Action<Entity>? OnTurnStart;
        public event Action<Entity>? OnTurnEnd;

        public override void Subscribe()
        {
            // Optionally subscribe to world or other systems
        }

        public void StartTurn()
        {
            IsActive = true;
            Printer.PrintTurnStartHeader(Owner);
            OnTurnStart?.Invoke(Owner);
        }

        public void EndTurn()
        {
            Printer.PrintTurnEndHeader(Owner);
            OnTurnEnd?.Invoke(Owner);
            IsActive = false;
        }
    }
}
