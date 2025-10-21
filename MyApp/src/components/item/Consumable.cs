namespace CBA
{
    public class Consumable(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            TrackSubscription<Action<Entity, Entity>>(
                h => Owner.GetComponent<Usable>().OnUseSuccess += h,
                h => Owner.GetComponent<Usable>().OnUseSuccess -= h,
                (_, _) => World.Instance.RemoveEntity(Owner)
            );
        }
    }
}