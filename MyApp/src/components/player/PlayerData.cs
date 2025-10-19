namespace CBA
{
    public class PlayerData(Entity owner, string name) : Component(owner)
    {
        public string Name { get; set; } = name;

        public override void ValidateDependencies()
        {
            if (Owner.Id.Category != EntityCategory.Player)
                throw new InvalidOperationException($"PlayerData was given to an invalid category of entity: {Owner.Id}.");
        }
        public override void Subscribe()
        {
            // Nothing to subscribe to for now
        }
    }
}
