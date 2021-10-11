using System.Reflection;
using GreatWorkIvory.Events;
using GreatWorkIvory.Utils;
using SecretHistories.UI;

namespace GreatWorkIvory
{
    public class GreatWorkAPI
    {
        public static readonly EventManager Events = new EventManager();

        public static void RegisterAssembly(Assembly a)
        {
            Events.RegisterGlobal(a);
            Events.Register(new AssemblySpecificEvents(a));
        }

        public static void RegisterCurrentAssembly()
        {
            RegisterAssembly(Assembly.GetAssembly(ReflectionUtils.GetCaller()));
        }

        public static void ReloadCompendium()
        {
            Watchman.Get<CompendiumLoader>().PopulateCompendium(Watchman.Get<Compendium>(),
                Watchman.Get<Config>().GetConfigValue("Culture"));
        }
    }
}