using System;

namespace GreatWork.Events
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeEvent : Attribute
    {
    }
}