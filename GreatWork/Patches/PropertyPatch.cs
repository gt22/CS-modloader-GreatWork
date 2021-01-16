using System.Collections;
using Assets.Core.Entities;
using Assets.Core.Fucine;
using GreatWork.Utils;

namespace GreatWork.Patches
{
    public static class PropertyPatch
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(AbstractEntity<Legacy>).Method("PopAllUnknownProperties"),
                HarmonyHolder.Wrap("Replacement")
            );
        }

        private static bool Replacement(ref Hashtable __result, Hashtable ___UnknownProperties)
        {
            __result = ___UnknownProperties;
            return false;
        }
    }
}