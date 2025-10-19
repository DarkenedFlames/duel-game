using System.Buffers;

namespace CBA
{
    public class CompletesItemSet(Entity owner, string setTag) : Component(owner)
    {
        public string SetTag { get; init; } = setTag;
        public bool SetActive { get; set; } = false;
        public event Action<Entity>? OnArmorSetCompleted;
        public event Action<Entity>? OnArmorSetBroken;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Item)
                throw new InvalidOperationException($"CompletesItemSet was given to an invalid category of entity: {Owner.Id}.");
            if (!Owner.HasComponent<Wearable>())
                throw new InvalidOperationException("CompletesItemSet requires Wearable.");
            if (Owner.GetComponent<ItemData>().Type != ItemType.Armor)
                throw new InvalidOperationException("CompletesItemSet requires ItemData.Type == ItemType.Armor.");
        }
        
        public override void Subscribe()
        {
            Owner.GetComponent<Wearable>().OnEquipSuccess += _ => CheckForArmorSetCompleted();
            Owner.GetComponent<Wearable>().OnUnequipSuccess += _ => CheckForArmorSetBroken();
        }

        private List<CompletesItemSet> GetSameSetEquipped()
        {
            Entity wearer = World.Instance.GetPlayerOf(Owner);

            // Find all equipped items on this player with the same set tag
            List<CompletesItemSet> sameSetEquipped = [.. World.Instance
                .GetAllForPlayer<Entity>(wearer, EntityCategory.Item, null, equipped: true)
                .Where(e => e.HasComponent<CompletesItemSet>())
                .Select(e => e.GetComponent<CompletesItemSet>())
                .Where(c => c.SetTag == SetTag)];

            return sameSetEquipped;
        }

        private void CheckForArmorSetCompleted()
        {
            List<CompletesItemSet> sameSetEquipped = GetSameSetEquipped();

            if (sameSetEquipped.Count == 3 && !SetActive)
            {
                foreach (var c in sameSetEquipped)
                    c.SetActive = true;

                OnArmorSetCompleted?.Invoke(Owner);
            }
        }

        private void CheckForArmorSetBroken()
        {
            List<CompletesItemSet> sameSetEquipped = GetSameSetEquipped();

            if (sameSetEquipped.Count >= 3 && SetActive)
            {
                foreach (var c in sameSetEquipped)
                    c.SetActive = false;

                SetActive = false;
                OnArmorSetBroken?.Invoke(Owner);
            }
        }
    }
}
