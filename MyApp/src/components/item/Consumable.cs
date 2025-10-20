namespace CBA
{
    public class Consumable(Entity owner) : Component(owner)
    {
        public override void Subscribe()
        {
            Owner.GetComponent<Usable>().OnUseSuccess += (_, _) => OnUseSuccess();
        }
        private void OnUseSuccess()
        {
            Printer.PrintItemConsumed(Owner);
            World.Instance.RemoveEntity(Owner);        
        }
    }
}