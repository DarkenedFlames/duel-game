using System;

namespace CBA
{
    public abstract class Component
    {
        public Entity Owner { get; }
        protected Component(Entity owner)
        {
            Owner = owner;
            Owner.AddComponent(this);
            Subscribe();
        }
        protected abstract void Subscribe();
    }

}