using System;

namespace GreatWorkIvory.Events
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeEvent : Attribute
    {
    }
}