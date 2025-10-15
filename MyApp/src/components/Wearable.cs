namespace CBA
{
    public class Wearable(Entity owner, EquipType equipType) : Component(owner)
    {
        public bool IsEquipped { get; set; } = false;
        public ItemType Type { get; init; }
        public EquipType EquipType { get; init; } = equipType;

        public event Action<Entity>? OnEquipSuccess;
        public event Action<Entity>? OnUnequipSuccess;
        public event Action<Entity>? OnUnequipFail;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item)
                throw new InvalidOperationException($"Wearable was given to an invalid category of entity: {Owner.Id}.");
        }
        public override void Subscribe()
        {
            // Optional subscriptions to world events
        }

        // Query for all items of the same ItemType and Owner and unequip them. Then, equip this item and fire OnEquipSuccess/Failed.
        public void TryEquip()
        {
            // Ensure we have ItemData for this item
            Entity player = Owner.GetComponent<ItemData>().PlayerEntity;

            // --- Find conflicting equipped item of the same type for this player ---
            Wearable? conflicting = World.Instance
                .GetItemsForPlayer(player)
                .Select(e => e.GetComponent<Wearable>())
                .Where(w => w != null &&
                            w.IsEquipped &&
                            w.EquipType == EquipType &&
                            w.Owner != Owner)
                .FirstOrDefault();

            // Unequip the conflicting item if it exists
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
