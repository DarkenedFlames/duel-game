namespace CBA
{
    public class Wearable(Entity owner, EquipType equipType) : Component(owner)
    {
        public bool IsEquipped { get; private set; } = false;
        public EquipType EquipType { get; init; } = equipType;
        public event Action<Entity>? OnEquipSuccess;
        public event Action<Entity>? OnUnequipSuccess;
        public event Action<Entity>? OnUnequipFail;

        public override void Subscribe(){}
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
            Printer.PrintItemEquipped(Owner);
            OnEquipSuccess?.Invoke(Owner);
        }

        // If item is equipped, unequip it. Fire OnUnequipSuccess/Failed event.
        public void TryUnequip()
        {
            if (IsEquipped)
            {
                IsEquipped = false;
                Printer.PrintItemEquipped(Owner);
                OnUnequipSuccess?.Invoke(Owner);
            }
            else
            {
                OnUnequipFail?.Invoke(Owner);
            }
        }



    }
}
