using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.CS.TabletopUI;
using Assets.TabletopUi.Scripts.Infrastructure.Modding;
using BepInEx;
using GreatWork.Events;
using GreatWork.Utils;
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
        }

        private static bool Prefix(ref string spriteResourceName)
        {
            spriteResourceName = spriteResourceName
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);
            return true;
        }
        
    }
}