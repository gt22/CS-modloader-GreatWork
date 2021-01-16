using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Assets.Core.Fucine;
using Assets.TabletopUi.Scripts.Infrastructure.Modding;
using GreatWork.Events;
using GreatWork.Events.EventTypes;
using GreatWork.Utils;
using HarmonyLib;

namespace GreatWork.Patches
{
    public static class CompendiumPatch
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(CompendiumLoader).Method("PopulateCompendium"),
                HarmonyHolder.Wrap("Prefix"),
                HarmonyHolder.Wrap("Postfix"),
                HarmonyHolder.Wrap("Transpiler")
            );
        }

        private static bool Prefix(ICompendium compendiumToPopulate, string forCultureId, ContentImportLog ____log)
        {
            GreatWorkAPI.Events.FireEvent(new CompendiumEvent.Begin(compendiumToPopulate, forCultureId, ____log));
            return true;
        }

        private static bool IsLdLoc(OpCode o)
        {
            return o.ToString().ToLower().Contains("ldloc");
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> original)
        {
            var ins = original.ToList();

            var loadCompendium = new CodeInstruction(OpCodes.Ldarg_1);
            var loadCulture = new CodeInstruction(OpCodes.Ldarg_2);
            var loadThis = new CodeInstruction(OpCodes.Ldarg_0);
            var loadLog = new CodeInstruction(OpCodes.Ldfld, typeof(CompendiumLoader).Field("_log"));

            var typeLoadSegment = ins.SkipWhile(x =>
                !(x.opcode == OpCodes.Newobj &&
                  ((ConstructorInfo) x.operand).DeclaringType == typeof(EntityTypeDataLoader))).ToList();
            var loadTypeDict = typeLoadSegment.First(x => IsLdLoc(x.opcode));
            var loadTypeList = typeLoadSegment.SkipWhile(x =>
                    !x.Calls(typeof(Dictionary<string, EntityTypeDataLoader>).Method("Add")))
                .First(x => IsLdLoc(x.opcode));

            var modLoadSegment = ins.SkipWhile(x => !x.Calls(typeof(ModManager).Method("GetEnabledMods"))).ToList();
            var loadContentLoader = modLoadSegment
                .SkipWhile(x => !x.Calls(typeof(DataFileLoader).Method("LoadFilesFromAssignedFolder")))
                .First(x => IsLdLoc(x.opcode));
            var loadLocLoader = modLoadSegment.SkipWhile(x => x != loadContentLoader)
                .SkipWhile(x => !x.Calls(typeof(DataFileLoader).Method("LoadFilesFromAssignedFolder")))
                .First(x => IsLdLoc(x.opcode));

            var inTypeSegment = false;
            foreach (var x in ins)
            {
                if (inTypeSegment && x.opcode == OpCodes.Ldarg_1)
                {
                    inTypeSegment = false;
                    foreach (var e in FireEvent(typeof(CompendiumEvent.TypeRegistry.Post),
                        true,
                        loadCompendium, loadCulture, loadThis, loadLog, loadTypeDict, loadTypeList))
                        yield return e;
                }

                yield return x;

                if (x.Calls(typeof(ModManager).Method("CatalogueMods")))
                    foreach (var e in FireEvent(typeof(CompendiumEvent.ModIndexing.Pre),
                        true,
                        loadCompendium, loadCulture, loadThis, loadLog, loadContentLoader, loadLocLoader))
                        yield return e;
                if (x.Calls(typeof(Assembly).Method("GetTypes")))
                {
                    foreach (var e in FireEvent(typeof(CompendiumEvent.ModIndexing.Post),
                        true,
                        loadCompendium, loadCulture, loadThis, loadLog, loadContentLoader, loadLocLoader))
                        yield return e;
                    foreach (var e in FireEvent(typeof(CompendiumEvent.TypeRegistry.Pre),
                        true,
                        loadCompendium, loadCulture, loadThis, loadLog, loadTypeDict, loadTypeList))
                        yield return e;

                    inTypeSegment = true;
                }
            }
        }

        private static IEnumerable<CodeInstruction> FireEvent(Type e, bool discard, params CodeInstruction[] argLoaders)
        {
            yield return new CodeInstruction(OpCodes.Ldsfld, typeof(GreatWorkAPI).Field("Events"));
            foreach (var loader in argLoaders) yield return loader;

            yield return new CodeInstruction(OpCodes.Newobj, e.GetConstructors()[0]);
            yield return new CodeInstruction(OpCodes.Call, typeof(EventManager).Method("FireEvent"));
            if (discard) yield return new CodeInstruction(OpCodes.Pop);
        }


        private static void Postfix(ICompendium compendiumToPopulate, string forCultureId, ContentImportLog ____log)
        {
            GreatWorkAPI.Events.FireEvent(new CompendiumEvent.End(compendiumToPopulate, forCultureId, ____log));
        }
    }
}