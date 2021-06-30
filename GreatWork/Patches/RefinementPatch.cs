using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Assets.Core;
using Assets.Core.Entities;
using Assets.Core.Services;
using Assets.CS.TabletopUI;
using GreatWork.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using OrbCreationExtensions;
using UnityEngine;

namespace GreatWork.Patches
{
    public class RefinementPatch
    {

        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(TokenDetailsWindow).Method("SetElementCard"),
                transpiler: HarmonyHolder.Wrap("RefineElementDescription")
            );
            HarmonyHolder.Harmony.Patch(
                typeof(TextRefiner).Method("RefineString"),
                transpiler: HarmonyHolder.Wrap("RefinementByCount")
            );
        }

        public static string GetRefinedDescription(Element elem, ElementStackToken token)
        {
            IAspectsDictionary aspects;
            if (token == null)
            {
                aspects = new AspectsDictionary();
                aspects[elem.Id] = 1;
            } else {
                aspects = token.GetAspects(true);
            }
            var refiner = new TextRefiner(aspects);
            return refiner.RefineString(elem.Description);
        }

        private static IEnumerable<CodeInstruction> RefineElementDescription(IEnumerable<CodeInstruction> original)
        {
            foreach (var ins in original)
            {
                if (ins.opcode == OpCodes.Callvirt && ((MethodInfo) ins.operand).Name == "get_Description")
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Call, typeof(RefinementPatch).Method("GetRefinedDescription"));
                }
                else
                {
                    yield return ins;
                }
            }
        }

        public static bool MatchAspectToRefinement(IAspectsDictionary aspects, string refinement)
        {
            var parts = refinement.Split('~');
            if (parts.Length == 1)
            {
                return aspects.ContainsKey(refinement);
            }
            return aspects.ContainsKey(parts[0]) && aspects[parts[0]] >= int.Parse(parts[1]);
        }

        private static IEnumerable<CodeInstruction> RefinementByCount(IEnumerable<CodeInstruction> original)
        {
            foreach (var ins in original)
            {
                if (ins.opcode == OpCodes.Callvirt && ((MethodInfo) ins.operand).Name == "ContainsKey")
                {
                    yield return new CodeInstruction(OpCodes.Call,
                        typeof(RefinementPatch).Method("MatchAspectToRefinement"));
                }
                else
                {
                    yield return ins;
                }
            }
        }
        
    }
}