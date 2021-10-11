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

        private static void Prefix(IEntityWithId __instance, bool ___Refined, Hashtable ___UnknownProperties, ContentImportLog log)
        {
            if (___Refined) return;
            if (!___UnknownProperties.ContainsKey("gwextensions")) return;
            if (___UnknownProperties["gwextensions"] is EntityData data)
            {
                Beachcomber.Load(__instance, data, log);
                ___UnknownProperties.Remove("gwextensions");
            }
        }
    }
}