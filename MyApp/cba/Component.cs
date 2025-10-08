using System;

namespace CBA
{
    public abstract class Component
    {
        protected Entity Entity { get; private set; } = null!;

        public virtual void Initialize(Entity entity)
        {
            Entity = entity;
            Subscribe();
        }

        protected virtual void Subscribe() { }
    }
}