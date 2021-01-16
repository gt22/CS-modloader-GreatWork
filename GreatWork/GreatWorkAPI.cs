using System.Reflection;
using GreatWork.Events;
using GreatWork.Utils;

namespace GreatWork
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
    }
}