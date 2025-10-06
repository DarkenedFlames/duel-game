using System;

namespace MyCBA
{
    public abstract class Component
    {
        public Entity? Owner { get; internal set; }
    }
}