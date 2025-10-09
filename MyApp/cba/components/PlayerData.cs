using System;

namespace CBA
{
    public class PlayerData : Component
    {
        public string Name { get; set; }

        public PlayerData(Entity owner, string name) : base(owner)
        {
            Name = name;
        }

        protected override void Subscribe()
        {
            // Nothing to subscribe to for now
        }
    }
}
