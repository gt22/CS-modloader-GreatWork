using System.Reflection;
using GreatWork.Events;
using GreatWork.Events.EventTypes;

namespace GreatWork
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