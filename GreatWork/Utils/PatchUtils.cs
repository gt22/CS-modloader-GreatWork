using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using GreatWork.Events;
using HarmonyLib;

namespace GreatWork.Utils
{
    public class PatchUtils
    {
        public static IEnumerable<CodeInstruction> FireEvent(Type e, bool discard, params CodeInstruction[] argLoaders)
        {
            yield return new CodeInstruction(OpCodes.Ldsfld, typeof(GreatWorkAPI).Field("Events"));
            foreach (var loader in argLoaders) yield return loader;

            yield return new CodeInstruction(OpCodes.Newobj, e.GetConstructors()[0]);
            yield return new CodeInstruction(OpCodes.Call, typeof(EventManager).Method("FireEvent"));
            if (discard) yield return new CodeInstruction(OpCodes.Pop);
        }
        
        public static bool IsLdLoc(OpCode o)
        {
            return o.ToString().ToLower().Contains("ldloc");
        }
    }
}