namespace GreatWork.Events.EventTypes
{
    public class ServiceInitializationEvent<T> : Event
    {
        public readonly T Service;

        public ServiceInitializationEvent(T service)
        {
            Service = service;
        }


        public class PreInit : ServiceInitializationEvent<T>
        {
            public PreInit(T service) : base(service)
            {
            }
        }

        public class PostInit : ServiceInitializationEvent<T>
        {
            public PostInit(T service) : base(service)
            {
            }
        }
    }
}