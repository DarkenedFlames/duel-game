using System;

namespace MyApp
{
    public class StatBoostPotion : Item
    {
        public StatBoostPotion(Player owner, string name, int staminaCost,
                                string? statName = null, int statBaseBoost = 0, float statModifierBoost = 1.0f,
                                string? resourceName = null, int resourceBaseBoost = 0, float resourceModifierBoost = 1.0f
                                ) : base(owner)
        {
            Name = name;
            StaminaCost = staminaCost;
            Type = ItemType.Consumable;
            Rarity = ItemRarity.Common;

            OnUsed += (item, target) =>
            {
                if (resourceModifierBoost != 1.0f && resourceName != null)
                    target.Resources.IncreaseMultiplier(resourceName, resourceModifierBoost);
                if (resourceBaseBoost != 0 && resourceName != null)
                    target.Resources.Change(resourceName, resourceBaseBoost);
                if (statModifierBoost != 1.0f && statName != null)
                    target.Stats.IncreaseModifier(statName, statModifierBoost);
                if (statBaseBoost != 0 && statName != null)
                    target.Stats.IncreaseBase(statName, statBaseBoost);
            };
        }
    }
}
