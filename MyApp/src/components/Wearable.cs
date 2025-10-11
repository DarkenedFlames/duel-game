using System;
using System.Linq;

namespace CBA
{
    public class Wearable(Entity owner, EquipType equipType) : Component(owner)
    {
        public bool IsEquipped { get; set; } = false;
        public ItemType Type { get; init; }
        public EquipType EquipType { get; init; } = equipType;

        public event Action<Entity>? OnEquipSuccess;
        public event Action<Entity>? OnEquipFail;
        public event Action<Entity>? OnUnequipSuccess;
        public event Action<Entity>? OnUnequipFail;

        public override void Subscribe()
        {
            // Optional subscriptions to world events
        }

        // Query for all items of the same ItemType and Owner and unequip them. Then, equip this item and fire OnEquipSuccess/Failed.
        public void TryEquip()
        {
            var itemData = Owner.GetComponent<ItemData>();
            if (itemData == null)
            {
                OnEquipFail?.Invoke(Owner);
                return;
            }

            // Find any other equipped item of the same type for this player
            var conflicting = World.Instance.GetEntitiesWith<Wearable>()
                .Select(e => e.GetComponent<Wearable>())
                .Where(w =>
                    w != null &&
                    w.IsEquipped &&
                    w.EquipType == EquipType &&
                    w.Owner != Owner &&
                    w.Owner.GetComponent<ItemData>()?.PlayerEntity == itemData.PlayerEntity)
                .FirstOrDefault();


            // If there’s a conflicting item, unequip it first
            conflicting?.TryUnequip();

            // Equip this item
            IsEquipped = true;
            OnEquipSuccess?.Invoke(Owner);
        }

        // If item is equipped, unequip it. Fire OnUnequipSuccess/Failed event.
        public void TryUnequip()
        {
            if (IsEquipped)
            {
                IsEquipped = false;
                OnUnequipSuccess?.Invoke(Owner);
            }
            else
            {
                OnUnequipFail?.Invoke(Owner);
            }
        }
    }
}
