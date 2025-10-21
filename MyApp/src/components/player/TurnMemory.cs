namespace CBA
{
    public class TurnMemory(Entity owner) : Component(owner)
    {
        private readonly Dictionary<string, int> _effectsApplied = [];

        public override void Subscribe()
        {
            TrackSubscription<Action<Entity>>(
                h => World.Instance.TurnManager.OnTurnStart += h,
                h => World.Instance.TurnManager.OnTurnStart -= h,
                turnTaker => { if (turnTaker == Owner) Clear(); }
            );

            World.Instance.OnEntityAdded += entity =>
            {
                if (entity.Id.Category == EntityCategory.Effect
                    && World.Instance.GetPlayerOf(entity) == Owner)
                    RecordEffectApplied(entity.Id.TypeId);
            };
            TrackSubscription<Action<Entity>>(
                h => World.Instance.OnEntityAdded += h,
                h => World.Instance.OnEntityAdded -= h,
                entity =>
                {
                    if (entity.Id.Category == EntityCategory.Effect && World.Instance.GetPlayerOf(entity) == Owner)
                        RecordEffectApplied(entity.Id.TypeId);
                }
            );
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