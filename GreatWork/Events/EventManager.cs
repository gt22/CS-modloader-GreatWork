using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GreatWork.Events.EventTypes;
using GreatWork.Events.Filters;
using GreatWork.Events.Links;
using GreatWork.Utils;
using Noon;

namespace GreatWork.Events
{
    public class EventManager
    {
        private readonly Dictionary<Type, List<Tuple<MethodInfo, object, IEnumerable<EventFilter>>>> _methods =
            new Dictionary<Type, List<Tuple<MethodInfo, object, IEnumerable<EventFilter>>>>();


        public void RegisterGlobal(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
                if (type.GetCustomAttribute(typeof(GwEventHandler)) != null)
                {
                    NoonUtility.Log("Registering " + type);
                    Register(type, null);
                }

            NoonUtility.Log("Event registry for assembly " + assembly.GetName().Name + " complete");
        }

        public void Register<T>(T instance = default)
        {
            Register(typeof(T), instance);
        }

        public void Register(Type t, object instance = default)
        {
            foreach (var mt in t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public |
                                            BindingFlags.NonPublic))
            {
                var sub = (SubscribeEvent) mt.GetCustomAttribute(typeof(SubscribeEvent));
                if (sub != null)
                {
                    var filters = Enumerable.Empty<EventFilter>();
                    var param = mt.GetParameters();
                    if (param.Length != 1)
                    {
                        NoonUtility.Log($"Event handlers should only exactly one parameter. {mt} has {param.Length}", 2,
                            VerbosityLevel.Significants);
                        continue;
                    }

                    var et = param[0].ParameterType;
                    var link = (EventLink) et.GetCustomAttribute(typeof(EventLink));
                    if (link != null) et = link.ForEvent;

                    if (!typeof(Event).IsAssignableFrom(et))
                    {
                        NoonUtility.Log($"Parameter of {mt} is of type {et}, which is not an event", 2,
                            VerbosityLevel.Significants);
                        continue;
                    }

                    if (!_methods.ContainsKey(et))
                        _methods[et] = new List<Tuple<MethodInfo, object, IEnumerable<EventFilter>>>();
                    _methods[et].Add(new Tuple<MethodInfo, object, IEnumerable<EventFilter>>(mt, instance, filters));
                }
            }
        }

        public bool FireEvent(Event e)
        {
            for (var et = e.GetType(); et != typeof(object) && et != null; et = et.BaseType)
                if (_methods.ContainsKey(et))
                    foreach (var (mt, instance, filters) in _methods[et])
                    {
                        var pass = filters.Aggregate(true, (current, filter) => current & filter.Accept(e));
                        if (!pass) continue;
                        var actualType = mt.GetParameters()[0].ParameterType;
                        object arg = e;
                        if (actualType != et)
                        {
                            var cur = et;
                            ConstructorInfo cons = null;
                            while (cons == null)
                            {
                                cons = actualType.Constructor(cur);
                                cur = et.BaseType;
                            }

                            var link = (IEventLink) cons.Invoke(new object[] {e});
                            if (!link.IsValid()) continue;
                            arg = link;
                        }

                        var res = mt.Invoke(instance, new[] {arg});
                        if (mt.ReturnType == typeof(bool))
                            if (!(bool) res)
                                return false;
                    }

            return true;
        }
    }
}