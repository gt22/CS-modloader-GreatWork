using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Assets.Core.Fucine;
using Assets.CS.TabletopUI;
using Assets.TabletopUi.Scripts.Infrastructure.Modding;
using GreatWork.Events;
using GreatWork.Events.EventTypes;
using GreatWork.Utils;
using HarmonyLib;
using Noon;
using UIWidgets;
using UnityEngine;

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
            HarmonyHolder.Harmony.Patch(
                typeof(ModManager).Method("GetCataloguedMods"),
                HarmonyHolder.Wrap("GetCataloguedMods")
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

        /*
         * Perform topological sort on mods to ensure order loading order is consistent with dependencies
         */
        private static bool GetCataloguedMods(ModManager __instance, ref IEnumerable<Mod> __result)
        {
            var modsById =
                (Dictionary<string, Mod>) typeof(ModManager).Property("_cataloguedMods").GetValue(__instance);
            var mods = modsById.Values;
            var outdeg = new Dictionary<string, int>();
            var revDep = new Dictionary<string, List<string>>();
            foreach (var m in mods)
            {
                foreach (var d in m.Dependencies.Select(d => d.ModId))
                {
                    outdeg.Compute(m.Id, (_, x) => x + 1);
                    revDep.ComputeIfAbsent(d, _ => new List<string>()).Add(m.Id);
                }
            }

            var freeMods = mods.Where(m => !outdeg.TryGetValue(m.Id, out var mo) || mo == 0)
                .Select(m => m.Id)
                .ToList();
            freeMods.Reverse(); // Reverse to compensate for removing from the end, to match original order, when no dependencies are present
            var ret = new List<Mod>();
            while (freeMods.Count > 0)
            {
                var m = freeMods.Pop();
                if (modsById.TryGetValue(m, out var mod))
                {
                    ret.Add(mod);
                }

                if (revDep.TryGetValue(m, out var rd))
                {
                    freeMods.AddRange(
                        from d in rd
                        where --outdeg[d] == 0
                        select d
                    );
                }
            }

            foreach (var m in outdeg.Where(m => m.Value > 0).Select(m => m.Key))
            {
                NoonUtility.Log($"[GreatWork] Cyclic dependency detected for mod {m}, loading them in some order", 2,
                    VerbosityLevel.Essential);
                ret.Add(modsById[m]);
            }

            __result = ret;
            return false;
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