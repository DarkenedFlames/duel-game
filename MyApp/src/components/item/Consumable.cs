namespace CBA
{
    public class Consumable(Entity owner) : Component(owner)
    {
        protected override void RegisterSubscriptions()
        {
            RegisterSubscription<Action<Entity, Entity>>(
                h => Owner.GetComponent<Usable>().OnUseSuccess += h,
                h => Owner.GetComponent<Usable>().OnUseSuccess -= h,
                (_, _) => World.Instance.RemoveEntity(Owner)
            );
        }
    }
}