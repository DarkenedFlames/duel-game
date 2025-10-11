namespace CBA
{
    [Flags]
    public enum EffectTrigger
    {
        None      = 0,
        OnUse     = 1 << 0,
        OnEquip   = 1 << 1,
        OnUnequip = 1 << 2
        // Future triggers: OnAttack, OnDefend, etc.
    }

    public class ModifiesEffects(Entity owner) : Component(owner)
    {
        // Map triggers to list of effect names
        public Dictionary<EffectTrigger, List<string>> TriggeredEffects { get; } = new();

        public event Action<Entity, Entity, string>? OnEffectApplied;

        public override void Subscribe()
        {
            var usable = Owner.GetComponent<Usable>();
            var wearable = Owner.GetComponent<Wearable>();

            usable?.OnUseSuccess += (item, target) =>
                ApplyByTrigger(EffectTrigger.OnUse, item, target);

            wearable?.OnEquipSuccess += item =>
            {
                var wearer = item.GetComponent<ItemData>()?.PlayerEntity;
                if (wearer != null) ApplyByTrigger(EffectTrigger.OnEquip, item, wearer);
            };

            wearable?.OnUnequipSuccess += item =>
            {
                var wearer = item.GetComponent<ItemData>()?.PlayerEntity;
                if (wearer != null) ApplyByTrigger(EffectTrigger.OnUnequip, item, wearer);
            };
        }

        private void ApplyByTrigger(EffectTrigger trigger, Entity item, Entity target)
        {
            if (!TriggeredEffects.TryGetValue(trigger, out var effects) || target == null) 
                return;

            var user = item.GetComponent<ItemData>()?.PlayerEntity;
            if (user == null) return;

            foreach (var effectName in effects)
            {
                if (trigger == EffectTrigger.OnUnequip)
                    RemoveEffect(effectName, target);
                else
                    ApplyEffect(effectName, user, target);
            }
        }

        private void ApplyEffect(string effectName, Entity user, Entity target)
        {
            var actualTarget = target ?? user; // fallback to user if target null

            EffectFactory.ApplyEffect(effectName, actualTarget);

            OnEffectApplied?.Invoke(Owner, actualTarget, effectName);
        }

        private void RemoveEffect(string effectName, Entity target)
        {
            foreach (var e in World.Instance.GetEntitiesWith<EffectData>())
            {
                var effectData = e.GetComponent<EffectData>();
                if (effectData?.Name == effectName &&
                    effectData.PlayerEntity == target)
                {
                    World.Instance.RemoveEntity(e);
                }
            }
        }
    }
}