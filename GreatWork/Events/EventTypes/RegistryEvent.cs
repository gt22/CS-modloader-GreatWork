namespace GreatWork.Events.EventTypes
{
    public class RegistryEvent : Event
    {
        public readonly object Item;

        public RegistryEvent(object item)
        {
            Item = item;
        }

        public class PreReg : RegistryEvent
        {
            public PreReg(object item) : base(item)
            {
            }
        }

        public class PostReg : RegistryEvent
        {
            public PostReg(object item) : base(item)
            {
            }
        }
    }
}