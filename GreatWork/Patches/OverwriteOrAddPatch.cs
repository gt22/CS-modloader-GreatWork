using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Assets.Core.Fucine.DataImport;
using HarmonyLib;
using Noon;
using GreatWork.Utils;

namespace GreatWork.Patches
{
	/*
	 * Class by SarahK
	 */
    class OverwriteOrAddPatch
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                AccessTools.Method(typeof(EntityData), "OverwriteOrAdd"),
                transpiler: new HarmonyMethod(typeof(OverwriteOrAddPatch).Method("ApplyPatchToFunction"))
            );
        }

        public static IEnumerable<CodeInstruction> ApplyPatchToFunction(IEnumerable<CodeInstruction> original)
        {
            foreach (CodeInstruction ins in original)
            {
                if (ins.opcode == OpCodes.Callvirt && ((MethodInfo) ins.operand).Name == "set_Item")
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(OverwriteOrAddPatch).Method("BetterOverwriteOrAdd"));
                }
                else
                {
                    yield return ins;
                }
            }
        }

        public static void BetterOverwriteOrAdd(Hashtable valuesTable, object key, object value)
        {
            if (valuesTable == null || key == null || !valuesTable.ContainsKey(key))
            {
                NoonUtility.Log(new NoonLogMessage("Couldn't validate valuesTable containing key!"));
                throw new Exception("ValuesTable didn't have the key!");
            }
            
            if (value == null)
            {
                return;
            }
            
            object oldValue = valuesTable[key];

            if (oldValue == null)
            {
                valuesTable.Add(key, value);
                return;
            }

            if (oldValue is int && value is string)
            {
                value = int.Parse(value.ToString());
            }

            if (oldValue.GetType() != value.GetType())
            {
                string message = "";
                message += "Tried to overwrite old value " + oldValue + " of type " + oldValue.GetType().FullName
                           + " in key " + key + " with new value "
                           + value + " of type " + value.GetType().FullName;
                NoonUtility.Log(new NoonLogMessage(message));
                
                throw new Exception("Tried to overwrite-or-add with a type other than the old type!");
            }
            
            switch (value)
            {
                case ArrayList list:
                    if (key.ToString().EndsWith("$prepend"))
                    {
                        list.AddRange((ArrayList) oldValue);
                        valuesTable[key] = list;
                    }
                    else if (key.ToString().Contains("$"))
                    {
                        ((ArrayList) oldValue).AddRange(list);
                        valuesTable[key] = oldValue;
                    }
                    else
                    {
                        valuesTable[key] = value;
                    }
                    break;
                case EntityData ed:
                    if (oldValue is EntityData oldEd)
                    {
                        if (key.ToString().Contains("$"))
                        {
                            foreach (string key2 in ed.ValuesTable.Keys)
                            {
                                oldEd.OverwriteOrAdd(key2, ed.GetEntityDataFromValueTable(key2));
                            }

                            valuesTable[key] = oldEd;
                        }
                        else
                        {
                            valuesTable[key] = ed;
                        }
                    }
                    else
                    {
                        throw new Exception("WHAT THE ACTUAL HELL JUST HAPPENED?!?!?");
                    }
                    break;
                case int v:
                    if (key.ToString().Contains("$"))
                        valuesTable[key] = (oldValue is int i ? i : 0) + v;
                    else
                    {
                        valuesTable[key] = v;
                    }
                    break;
                case double v:
                    if (key.ToString().Contains("$"))
                        valuesTable[key] = (oldValue is double d ? d : 0) + v;
                    else
                    {
                        valuesTable[key] = v;
                    }
                    break;
                default:
                    valuesTable[key] = value;
                    break;
            }
        }
    }
}