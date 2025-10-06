using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApp
{
    public class ActiveEffects
    {
        private readonly List<Effect> _effects = new();

        public IReadOnlyList<Effect> Effects => _effects.AsReadOnly();

        // Intent
        public event Action<Effect>? OnEffectAddRequest;

        // Resolution
        public event Action<Effect>? OnEffectAdded;
        public event Action<Effect>? OnEffectStacked;
        public event Action<Effect>? OnEffectRemoved;
        public event Action<Effect>? OnEffectTicked;

        public bool HasEffect<T>() where T : Effect => _effects.Any(e => e is T);
        public T? GetEffect<T>() where T : Effect => _effects.OfType<T>().FirstOrDefault();

        // Fires onEffectAddRequest
        // Fired by external callers
        public void RequestAddEffect(Effect effect)
        {
            OnEffectAddRequest?.Invoke(effect);
            AddEffect(effect);
        }

        // Fires onEffectAdded or onEffectStacked as appropriate
        private void AddEffect(Effect newEffect)
        {
            var existing = _effects.FirstOrDefault(e => e.GetType() == newEffect.GetType());
            if (existing != null)
            {
                StackingType stacking = existing.StackingType;
                switch (stacking)
                {
                    case StackingType.RefreshOnly:
                        existing.RemainingDuration = existing.MaximumDuration;
                        OnEffectStacked?.Invoke(existing);
                        break;

                    case StackingType.AddStack:
                        if (existing.RemainingStacks < existing.MaximumStacks)
                            existing.RemainingStacks++;
                        existing.RemainingDuration = existing.MaximumDuration;
                        OnEffectStacked?.Invoke(existing);
                        break;

                    case StackingType.Ignore:
                        return;
                }
            }
            else
            {
                _effects.Add(newEffect);
                OnEffectAdded?.Invoke(newEffect);
            }
        }

        // Fires onEffectRemoved if effect was present
        public void RemoveEffect(Effect effect)
        {
            if (_effects.Remove(effect))
            {
                OnEffectRemoved?.Invoke(effect);
            }
        }

        // Fires onEffectTicked for each effect, and removes expired effects
        public void TickAll()
        {
            foreach (var effect in _effects.ToList())
            {
                effect.RemainingDuration--;
                OnEffectTicked?.Invoke(effect);

                if (effect.RemainingDuration <= 0)
                    RemoveEffect(effect);
            }
        }
    }
}
