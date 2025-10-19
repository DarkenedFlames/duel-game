namespace CBA
{
    public class Wearable(Entity owner, EquipType equipType, string? setTag = null) : Component(owner)
    {
        public bool IsEquipped { get; private set; } = false;
        public EquipType EquipType { get; init; } = equipType;
        public string? SetTag { get; init; } = setTag;
        public bool SetActive { get; set; } = false;

        public event Action<Entity>? OnEquipSuccess;
        public event Action<Entity>? OnUnequipSuccess;
        public event Action<Entity>? OnUnequipFail;
        public event Action<Entity>? OnArmorSetCompleted;
        public event Action<Entity>? OnArmorSetBroken;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item)
                throw new InvalidOperationException($"Wearable was given to an invalid category of entity: {Owner.Id}.");
        }
        public override void Subscribe()
        {
            OnEquipSuccess += Printer.PrintItemEquipped;
            OnUnequipSuccess += Printer.PrintItemUnequipped;
        }

        // Query for all items of the same ItemType and Owner and unequip them. Then, equip this item and fire OnEquipSuccess/Failed.
        public void TryEquip()
        {
            // Ensure we have ItemData for this item
            Entity player = World.Instance.GetPlayerOf(Owner);

            // --- Find conflicting equipped item of the same type for this player ---
            Wearable? conflicting = World.Instance
                .GetAllForPlayer<Entity>(player, EntityCategory.Item, null, equipped: true)
                .Where(e => e != Owner)
                .Select(e => e.GetComponent<Wearable>())
                .Where(w => w.EquipType == EquipType)
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

        private void CheckForArmorSetCompleted()
        {
            Entity wearer = World.Instance.GetPlayerOf(Owner);

            // Find all equipped items on this player with the same set tag
            var sameSetEquipped = World.Instance
                .GetAllForPlayer<Entity>(wearer, EntityCategory.Item, null, equipped: true)
                .Select(e => e.GetComponent<Wearable>())
                .Where(w => w.SetTag == SetTag)
                .ToList();

            // If 3 pieces with this tag are equipped, mark them active and fire event (only on this one)
            if (sameSetEquipped.Count == 3)
            {
                foreach (var w in sameSetEquipped)
                    w.SetActive = true;

                OnArmorSetCompleted?.Invoke(Owner);
            }
        }

        private void HandleArmorSetBroken(Entity player)
        {
            var sameSetEquipped = World.Instance
                .GetAllForPlayer<Entity>(player, EntityCategory.Item, null, equipped: true)
                .Select(e => e.GetComponent<Wearable>())
                .Where(w => w.SetTag == SetTag)
                .ToList();

            foreach (var w in sameSetEquipped)
                w.SetActive = false;

            SetActive = false;
            OnArmorSetBroken?.Invoke(player);
        }

    }
}
