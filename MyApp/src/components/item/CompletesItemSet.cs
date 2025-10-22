namespace CBA
{
    public class CompletesItemSet(Entity owner, string setTag) : Component(owner)
    {
        public string SetTag { get; init; } = setTag;
        public bool SetActive { get; set; } = false;
        public event Action<Entity>? OnArmorSetCompleted;
        public event Action<Entity>? OnArmorSetBroken;
        
        protected override void RegisterSubscriptions()
        {
            RegisterSubscription<Action<Entity>>(
                h => Owner.GetComponent<Wearable>().OnEquipSuccess += h,
                h => Owner.GetComponent<Wearable>().OnEquipSuccess -= h,
                _ => CheckForArmorSetCompleted()
            );
            RegisterSubscription<Action<Entity>>(
                h => Owner.GetComponent<Wearable>().OnUnequipSuccess += h,
                h => Owner.GetComponent<Wearable>().OnUnequipSuccess -= h,
                _ => CheckForArmorSetBroken()
            );
        }
        private List<CompletesItemSet> GetSameSetEquipped()
        {
            Entity wearer = World.GetPlayerOf(Owner);

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

            if (sameSetEquipped.Count < 3 && SetActive)
            {
                foreach (var c in sameSetEquipped)
                    c.SetActive = false;

                SetActive = false;
                OnArmorSetBroken?.Invoke(Owner);
            }
        }
    }
}
