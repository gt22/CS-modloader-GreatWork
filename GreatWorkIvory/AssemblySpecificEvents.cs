using System.Reflection;
using GreatWorkIvory.Events;
using GreatWorkIvory.Events.EventTypes;

namespace GreatWorkIvory
{
    public class AssemblySpecificEvents
    {
        private readonly Assembly _a;

        public AssemblySpecificEvents(Assembly a)
        {
            _a = a;
        }

        [SubscribeEvent]
        private void Comp(CompendiumEvent.TypeRegistry.Post e)
        {
            foreach (var type in _a.GetTypes())
            {
                e.TryAddEntityType(type);
            }
        }
    }
}