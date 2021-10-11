using GreatWorkIvory.Events.EventTypes;

namespace GreatWorkIvory.Events.Filters
{
    public abstract class EventFilter
    {
        public abstract bool Accept(Event e);
    }
}