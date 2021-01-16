using GreatWork.Events.EventTypes;

namespace GreatWork.Events.Filters
{
    public abstract class EventFilter
    {
        public abstract bool Accept(Event e);
    }
}