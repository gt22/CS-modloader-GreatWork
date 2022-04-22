using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GreatWorkIvory.Events.EventTypes;
using GreatWorkIvory.Utils;
using HarmonyLib;
using SecretHistories.Constants.Modding;
using SecretHistories.Fucine;

namespace GreatWorkIvory.Patches
{
    public static class CompendiumPatch
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(CompendiumLoader).Method("PopulateCompendium"),
                HarmonyHolder.Wrap("PopulateCompendiumPrefix"),
                HarmonyHolder.Wrap("PopulateCompendiumPostfix"),
                HarmonyHolder.Wrap("PopulateCompendiumTranspiler")
            );
        }

        public class DependencyException : Exception
        {
            public DependencyException()
            {
            }

            public DependencyException(string message) : base(message)
            {
            }

            public DependencyException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        private static void PopulateCompendiumPrefix(Compendium compendiumToPopulate, string forCultureId, ContentImportLog ____log)
        {
            GreatWorkApi.Events.FireEvent(new CompendiumEvent.Begin(compendiumToPopulate, forCultureId, ____log));
        }

        private static IEnumerable<CodeInstruction> PopulateCompendiumTranspiler(IEnumerable<CodeInstruction> original)
        {
            var ins = original.ToList();

            var loadCompendium = new CodeInstruction(OpCodes.Ldarg_1);
            var loadCulture = new CodeInstruction(OpCodes.Ldarg_2);
            var loadThis = new CodeInstruction(OpCodes.Ldarg_0);
            var loadLog = new CodeInstruction(OpCodes.Ldfld, typeof(CompendiumLoader).Field("_log"));
            var loadContentLoader = new CodeInstruction(OpCodes.Ldfld, typeof(CompendiumLoader).Field("modContentLoaders"));
            var loadLocLoader = new CodeInstruction(OpCodes.Ldfld, typeof(CompendiumLoader).Field("modContentLoaders"));

            var typeLoadSegment = ins.SkipWhile(x =>
                !(x.opcode == OpCodes.Newobj &&
                  ((ConstructorInfo) x.operand).DeclaringType == typeof(EntityTypeDataLoader))).ToList();
            var loadTypeDict = typeLoadSegment.First(x => PatchUtils.IsLdLoc(x.opcode));
            var loadTypeList = typeLoadSegment.SkipWhile(x =>
                    !x.Calls(typeof(Dictionary<string, EntityTypeDataLoader>).Method("Add")))
                .First(x => PatchUtils.IsLdLoc(x.opcode));
            
            var inTypeSegment = false;
            foreach (var x in ins)
            {
                if (inTypeSegment && x.opcode == OpCodes.Ldarg_1)
                {
                    inTypeSegment = false;
                    foreach (var e in PatchUtils.FireEvent(typeof(CompendiumEvent.TypeRegistry.Post),
                        true,
                        loadCompendium, loadCulture, loadThis, loadLog, loadTypeDict, loadTypeList))
                        yield return e;
                }

                if (x.Calls(typeof(CompendiumLoader).Method("LoadModsToCompendium")))
                {
                    foreach (var e in PatchUtils.FireEvent(typeof(CompendiumEvent.ModIndexing.Pre),
                        true,
                        loadCompendium, loadCulture, loadThis, loadLog, loadThis, loadContentLoader, loadThis, loadLocLoader))
                        yield return e;
                }
                
                yield return x;
                
                if (x.Calls(typeof(CompendiumLoader).Method("LoadModsToCompendium")))
                {
                    foreach (var e in PatchUtils.FireEvent(typeof(CompendiumEvent.ModIndexing.Post),
                        true,
                        loadCompendium, loadCulture, loadThis, loadLog, loadThis, loadContentLoader, loadThis, loadLocLoader))
                        yield return e;
                }

                if (x.Calls(typeof(Assembly).Method("GetTypes")))
                {
                    foreach (var e in PatchUtils.FireEvent(typeof(CompendiumEvent.TypeRegistry.Pre),
                        true,
                        loadCompendium, loadCulture, loadThis, loadLog, loadTypeDict, loadTypeList))
                        yield return e;

                    inTypeSegment = true;
                }
            }
        }

        


        private static void PopulateCompendiumPostfix(Compendium compendiumToPopulate, string forCultureId, ContentImportLog ____log)
        {
            GreatWorkApi.Events.FireEvent(new CompendiumEvent.End(compendiumToPopulate, forCultureId, ____log));
        }
    }
}