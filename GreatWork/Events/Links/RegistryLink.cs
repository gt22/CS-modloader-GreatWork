using GreatWork.Events.EventTypes;

namespace GreatWork.Events.Links
{
    [EventLink(typeof(RegistryEvent))]
    public class RegistryLink<T> : IEventLink where T : class
    {
        private readonly object _item;
        public readonly T Item;

        public RegistryLink(RegistryEvent e)
        {
            _item = e.Item;
            if (_item is T r) Item = r;
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