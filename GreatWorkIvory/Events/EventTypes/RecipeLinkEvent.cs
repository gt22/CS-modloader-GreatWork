using SecretHistories.Core;
using SecretHistories.Entities;

namespace GreatWorkIvory.Events.EventTypes
{
    public class RecipeLinkEvent : Event
    {
        public readonly RecipeConductor Conductor;
        public readonly Recipe Source;
        public readonly AspectsInContext Aspects;
        public Recipe Linked;

        public RecipeLinkEvent(RecipeConductor conductor, Recipe source, AspectsInContext aspects, Recipe linked)
        {
            Conductor = conductor;
            Source = source;
            Aspects = aspects;
            Linked = linked;
        }
    }
}