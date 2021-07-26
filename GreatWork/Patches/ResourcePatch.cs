using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using Assets.CS.TabletopUI;
using Assets.TabletopUi.Scripts.Infrastructure.Modding;
using BepInEx;
using GreatWork.Events;
using GreatWork.Utils;
using HarmonyLib;
using UnityEngine;

namespace GreatWork.Patches
{
    public static class ResourcePatch
    {

        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(ModManager).Method("GetSprite"),
                HarmonyHolder.Wrap("Prefix")
            );
            HarmonyHolder.Harmony.Patch(
                    typeof(ModManager).Method("TryLoadAllImages"),
                    transpiler: HarmonyHolder.Wrap("DirFixer")
            );
        }

        private static bool Prefix(ref string spriteResourceName)
        {
            spriteResourceName = spriteResourceName
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);
            return true;
        }
        
        private static IEnumerable<CodeInstruction> DirFixer(IEnumerable<CodeInstruction> original)
        {
            foreach (var ins in original)
            {
                if (ins.opcode == OpCodes.Ldstr && (string) ins.operand == "images\\")
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "images" + Path.DirectorySeparatorChar);
                } else {
                    yield return ins;
                }
            }
        }
    }
}