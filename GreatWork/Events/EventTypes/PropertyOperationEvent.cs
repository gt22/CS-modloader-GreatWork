using Assets.Core.Fucine;
using Assets.Core.Fucine.DataImport;

namespace GreatWork.Events.EventTypes
{
    public class PropertyOperationEvent : Event
    {
        public readonly EntityMod Source;
        public readonly EntityData Target;
        public readonly ContentImportLog Log;
        public readonly string Property;
        public readonly string Operation;

        public PropertyOperationEvent(EntityMod source, ContentImportLog log, EntityData target, string[] prop)
        {
            Source = source;
            Target = target;
            Log = log;
            Property = prop[0];
            Operation = prop[1];
        }
    }
}