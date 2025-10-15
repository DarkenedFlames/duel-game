namespace CBA
{
    public abstract class Component
    {
        public Entity Owner { get; }
        protected Component(Entity owner)
        {
            Owner = owner;
            Owner.AddComponent(this);
        }
        public abstract void Subscribe();
        public abstract void ValidateDependencies();
    }

}