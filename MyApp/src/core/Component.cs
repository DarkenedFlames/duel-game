namespace CBA
{
    public abstract class Component(Entity owner)
    {
        public Entity Owner { get; } = owner;
        public abstract void Subscribe();
    }
}