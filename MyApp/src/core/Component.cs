namespace CBA
{
    public abstract class Component(Entity owner)
    {
        private Entity? _owner = owner;
        public Entity Owner => _owner!;
        private readonly List<(Delegate handler, Action<Delegate> unsubscribe)> _subscriptions = [];

        public virtual void Subscribe() { }
        public virtual void Unsubscribe()
        {
            foreach (var (handler, remove) in _subscriptions)
                remove(handler);
            _subscriptions.Clear();
        }
        protected void TrackSubscription<T>(Action<T> subscribe, Action<T> unsubscribe, T handler) where T : Delegate
        {
            subscribe(handler);
            _subscriptions.Add((handler, d => unsubscribe((T)d)));
        }

        internal void DetachOwner() => _owner = null;
    }
}