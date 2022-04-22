using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using GreatWorkIvory.Utils;
using HarmonyLib;
using System.Reflection.Emit;
using GreatWorkIvory.Fucine;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Patches
{
    public class DictOfEntitiesPatch
    {

        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(FucineDict).Method("CreateImporterInstance"),
                HarmonyHolder.Wrap("CreateImporterInstance")
            );
        }

        public static bool CreateImporterInstance(ref AbstractImporter __result)
        {
            __result = new BetterDictImporter();
            return false;
        }
    }
}