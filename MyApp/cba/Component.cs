using System;

namespace MyApp
{
    public abstract class Component
    {
        public Entity Owner { get; internal set; }
    }
}