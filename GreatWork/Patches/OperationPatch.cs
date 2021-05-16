using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Assets.Core.Fucine;
using Assets.TabletopUi.Scripts.Services;
using GreatWork.Events.EventTypes;
using GreatWork.Utils;
using HarmonyLib;

namespace GreatWork.Patches
{
    public class OperationPatch
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(EntityMod).Method("ProcessPropertyOperationsFromEntityMod"),
                transpiler: HarmonyHolder.Wrap("Transpiler")
            );
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> original)
        {
            var ins = original.ToList();
            var propArray = ins.SkipWhile(x => !x.Calls(typeof(string).Method("Split"))).First(x => PatchUtils.IsLdLoc(x.opcode));
            var endLabel = ins
                .SkipWhile(x =>
                    !x.Calls(typeof(ArrayList).Method("AddRange")))
                .SkipWhile(x => x.opcode != OpCodes.Br).First().operand;
            var patched = false;
            foreach (var i in ins)
            {
                if (!patched && i.opcode == OpCodes.Ldarg_2)
                {
                    patched = true;
                    foreach (var e in PatchUtils.FireEvent(typeof(PropertyOperationEvent), false,
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Ldarg_2),
                        propArray
                        ))
                    {
                        yield return e;
                    }

                    yield return new CodeInstruction(OpCodes.Brfalse, endLabel);
                }
                yield return i;
                
            }
        }
    }
}