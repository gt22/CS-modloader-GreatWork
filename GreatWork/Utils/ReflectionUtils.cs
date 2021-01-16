using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace GreatWork.Utils
{
    public static class ReflectionUtils
    {
        public static MethodInfo Method(this Type t, string name)
        {
            return AccessTools.Method(t, name);
        }

        public static FieldInfo Field(this Type t, string name)
        {
            return AccessTools.Field(t, name);
        }

        public static ConstructorInfo Constructor(this Type t, params Type[] p)
        {
            return AccessTools.Constructor(t, p);
        }

        public static Type GetCaller(Type startAt)
        {
            var st = new StackTrace(false);
            if (startAt == null) startAt = st.GetFrame(0).GetMethod().DeclaringType;
            return st.GetFrames().SkipWhile(f => f.GetMethod().DeclaringType != startAt)
                .First(f => f.GetMethod().DeclaringType != startAt).GetMethod().DeclaringType;
        }

        public static Type GetSelf()
        {
            return GetCaller(null);
        }

        public static Type GetCaller()
        {
            return GetCaller(GetSelf());
        }
    }
}