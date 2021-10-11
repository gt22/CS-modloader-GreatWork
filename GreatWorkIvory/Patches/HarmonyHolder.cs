using System;
using GreatWorkIvory.Utils;
using HarmonyLib;

namespace GreatWorkIvory.Patches
{
    public static class HarmonyHolder
    {
        public static readonly Harmony Harmony = new Harmony("greatwork.patches");

        public static HarmonyMethod Wrap(string name)
        {
            return new HarmonyMethod(ReflectionUtils.GetCaller().Method(name));
        }

        public static HarmonyMethod Wrap(string name, params Type[] t)
        {
            return new HarmonyMethod(ReflectionUtils.GetCaller().Method(name).MakeGenericMethod(t));
        }

        public static HarmonyMethod Wrap<T>(string name)
        {
            return Wrap(name, typeof(T));
        }
    }
}