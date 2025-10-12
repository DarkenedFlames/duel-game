namespace CBA
{
    public class TakesTurns(Entity owner) : Component(owner)
    {
        private static int _nextTurnOrder = 0;
        public int TurnOrder { get; } = _nextTurnOrder++;

        public event Action<Entity>? OnTurnStart;
        public event Action<Entity>? OnTurnEnd;

        public override void Subscribe()
        {
            // Optional subscription logic if needed
        }

        public void StartTurn()
        {
            Printer.PrintTurnStartHeader(Owner);
            OnTurnStart?.Invoke(Owner);
        }

        public void EndTurn()
        {
            Printer.PrintTurnEndHeader(Owner);
            OnTurnEnd?.Invoke(Owner);
        }
    }
}
