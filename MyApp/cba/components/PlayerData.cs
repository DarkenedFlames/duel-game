using System;

namespace CBA
{
    public class PlayerData(Entity owner, string name) : Component(owner)
    {
        public string Name { get; set; } = name;

        public override void Subscribe()
        {
            // Nothing to subscribe to for now
        }
    }
}
