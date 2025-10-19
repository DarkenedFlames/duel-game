namespace CBA
{
    public class Wearable(Entity owner, EquipType equipType) : Component(owner)
    {
        public bool IsEquipped { get; private set; } = false;
        public EquipType EquipType { get; init; } = equipType;
        public bool EquippedThisTurn { get; private set; } = false;

        public event Action<Entity>? OnEquipSuccess;
        public event Action<Entity>? OnUnequipSuccess;
        public event Action<Entity>? OnUnequipFail;
        public event Action<Entity>? OnFirstEquipPerTurn;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item)
                throw new InvalidOperationException($"Wearable was given to an invalid category of entity: {Owner.Id}.");
        }
        public override void Subscribe()
        {
            OnEquipSuccess += Printer.PrintItemEquipped;
            OnUnequipSuccess += Printer.PrintItemUnequipped;
            World.Instance.TurnManager.OnTurnStart += _ => EquippedThisTurn = false;
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

            if (!EquippedThisTurn)
            {
                EquippedThisTurn = true;
                OnFirstEquipPerTurn?.Invoke(Owner);      
            }

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
