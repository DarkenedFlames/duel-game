namespace CBA
{
    public class TurnMemory(Entity owner) : Component(owner)
    {
        private readonly Dictionary<string, int> _effectsApplied = new();

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Player)
                throw new InvalidOperationException("TurnMemory given to a non-player.");
        }

        public override void Subscribe()
        {
            World.Instance.TurnManager.OnTurnStart += turnTaker => { if (turnTaker == Owner) Clear(); };
            World.Instance.OnEntityAdded += entity =>
            {
                if (entity.Id.Category == EntityCategory.Effect && World.Instance.GetPlayerOf(entity) == Owner)
                    RecordEffectApplied(entity.Id.TypeId);
            };
        }
		
        public void RecordEffectApplied(string effectTypeId)
        {
            if (!_effectsApplied.ContainsKey(effectTypeId))
                _effectsApplied[effectTypeId] = 0;
            _effectsApplied[effectTypeId]++;
        }

        public int GetEffectsAppliedThisTurn(string effectTypeId)
        {
            return _effectsApplied.TryGetValue(effectTypeId, out int count) ? count : 0;
        }

        public void Clear()
        {
            _effectsApplied.Clear();
        }
    }
}