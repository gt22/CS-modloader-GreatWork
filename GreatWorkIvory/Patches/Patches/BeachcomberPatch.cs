using System;
using System.Collections;
using GreatWorkIvory.Utils;
using SecretHistories.Entities;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Patches
{
    public static class BeachcomberPatch
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(AbstractEntity<Element>).Method("OnPostImport"),
                HarmonyHolder.Wrap("Prefix")
            );
        }

        private static void Prefix(IEntityWithId __instance, bool ___Refined, Hashtable ___UnknownProperties,
            ContentImportLog log)
        {
            if (___Refined) return;
            Beachcomber.Load(__instance, ___UnknownProperties, log);
        }
    };
}