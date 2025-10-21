namespace CBA
{
    public abstract class Component(Entity owner)
    {
        private Entity? _owner = owner;
        public Entity Owner => _owner!;

        private readonly List<(Delegate handler, Action<Delegate> add, Action<Delegate> remove)> _subscriptions = [];

        protected virtual void RegisterSubscriptions() { }

        public virtual void Subscribe()
        {
            _subscriptions.Clear();
            RegisterSubscriptions();

            foreach (var (handler, add, _) in _subscriptions)
                add(handler);
        }

        public virtual void Unsubscribe()
        {
            foreach (var (handler, _, remove) in _subscriptions)
                remove(handler);
            _subscriptions.Clear();
        }

        protected void RegisterSubscription<T>(Action<T> add, Action<T> remove, T handler) where T : Delegate
        {
            _subscriptions.Add((handler, d => add((T)d), d => remove((T)d)));
        }

        internal void DetachOwner() => _owner = null;
    }
}
