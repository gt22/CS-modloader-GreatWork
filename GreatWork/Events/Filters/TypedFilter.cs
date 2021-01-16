using GreatWork.Events.EventTypes;

namespace GreatWork.Events.Filters
{
    public class TypedFilter<T> : EventFilter where T : Event
    {
        public override bool Accept(Event e)
        {
            return e is T et && AcceptTyped(et);
        }

        protected virtual bool AcceptTyped(T e)
        {
            return true;
        }
    }
}