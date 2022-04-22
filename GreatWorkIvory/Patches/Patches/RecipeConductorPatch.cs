using GreatWorkIvory.Events.EventTypes;
using GreatWorkIvory.Utils;
using SecretHistories.Core;
using SecretHistories.Entities;

namespace GreatWorkIvory.Patches
{
    public class RecipeConductorPatch
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(RecipeConductor).Method("GetLinkedRecipe"),
                postfix: HarmonyHolder.Wrap("Postfix")
            );
        }

        public static void Postfix(RecipeConductor __instance, AspectsInContext ____aspectsInContext, Recipe currentRecipe, ref Recipe __result)
        {
            RecipeLinkEvent e = new RecipeLinkEvent(__instance, currentRecipe, ____aspectsInContext, __result);
            GreatWorkApi.Events.FireEvent(e);
            __result = e.Linked;
        }
    }
}