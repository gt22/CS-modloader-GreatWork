using System;

namespace GreatWorkIvory.Events.Links
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventLink : Attribute
    {
        public readonly Type ForEvent;

        public EventLink(Type forEvent)
        {
            ForEvent = forEvent;
        }
    }

    public interface IEventLink
    {
        bool IsValid();
    }
}