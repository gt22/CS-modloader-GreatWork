using System.Collections.Generic;
using System.Reflection.Emit;
using Assets.TabletopUi.Scripts.Infrastructure;
using Assets.TabletopUi.Scripts.Infrastructure.Modding;
using GreatWork.Utils;
using HarmonyLib;
namespace GreatWork.Patches
{
    public static class ModPatch
    {

        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(Mod).Method("PopulateFromSynopsis"),
                transpiler: HarmonyHolder.Wrap("PopulateFromSynopsisTranspile")
            );
            HarmonyHolder.Harmony.Patch(
                typeof(SteamworksStorefrontClientProvider).Method("StartItemUpdate"),
                transpiler: HarmonyHolder.Wrap("StartItemUpdateTranspile")
            );
        }
        
        private static IEnumerable<CodeInstruction> PopulateFromSynopsisTranspile(IEnumerable<CodeInstruction> original)
        {
            var descCount = 0;
            foreach (var ins in original)
            {
                if (ins.opcode == OpCodes.Ldstr && (string) ins.operand == "description" && ++descCount == 2)
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "description_long");
                }
                else
                {
                    yield return ins;
                }
            }
        }

        private static IEnumerable<CodeInstruction> StartItemUpdateTranspile(IEnumerable<CodeInstruction> original)
        {
            foreach (var ins in original)
            {
                if (ins.Calls(typeof(Mod).Method("get_Description")))
                {
                    yield return new CodeInstruction(OpCodes.Call,
                        typeof(MiscUtils).Method("GetLongOrNormalDescription"));
                }
                else
                {
                    yield return ins;
                }
            }
        }
    }
}