using GreatWorkIvory.Events.EventTypes;

namespace GreatWorkIvory.Events.Links
{
    [EventLink(typeof(RegistryEvent))]
    public class RegistryLink<T> : IEventLink where T : class
    {
        private readonly object _item;

        public T Item => (T) _item;

        public RegistryLink(RegistryEvent e)
        {
            _item = e.Item;
        }

        public bool IsValid()
        {
            return _item is T;
        }

        [EventLink(typeof(RegistryEvent.PreReg))]
        public class PreReg : RegistryLink<T>
        {
            public PreReg(RegistryEvent e) : base(e)
            {
            }
        }

        [EventLink(typeof(RegistryEvent.PostReg))]
        public class PostReg : RegistryLink<T>
        {
            public PostReg(RegistryEvent e) : base(e)
            {
            }
        }
    }
}