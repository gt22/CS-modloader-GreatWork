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
            
            // Note: null data and empty data are not the same thing.
            if (value == null)
            {
                return;
            }
            
            object oldValue = valuesTable[key];
            
            switch (oldValue)
            {
                case null:
                   valuesTable.Add(key, value);
                   return;
                case int _ when value is string && int.TryParse(value.ToString(), out int _):
                   value = int.Parse(value.ToString());
                   break;
                case double _ when value is string && double.TryParse(value.ToString(), out double _):
                   value = double.Parse(value.ToString());
                   break;
            }

            // We want to support the cases of replacing strings with numbers and numbers with strings.
            // We also want to support the cases of replacing string-XTriggers with an ArrayList of actual XTrigger data.
            // Otherwise, it may be best to notify the modder that something went wrong.
            // In the worst case, we'll need to patch this function with another exception-case.
            if (oldValue.GetType() != value.GetType()
                && !(oldValue is string && (value is int || value is double || value is ArrayList))
                && !((oldValue is int || oldValue is double) && value is string)
                )
            {
                string message = "";
                message += "Tried to overwrite old value '" + oldValue + "' of type '" + oldValue.GetType().FullName
                           + "' in key '" + key + "' with new value '"
                           + value + "' of type " + value.GetType().FullName;
                NoonUtility.Log(new NoonLogMessage(message));
                
                throw new Exception("Tried to overwrite-or-add with a type other than the old type!");
            }

            // Here's where the magic happens.
            switch (value)
            {
                case ArrayList list:
                    if (key.ToString().EndsWith("$prepend"))
                    {
                        // Prepend new value to old list
                        list.AddRange((ArrayList) oldValue);
                        valuesTable[key] = list;
                    }
                    else if (key.ToString().EndsWith("$append")
                             || key.ToString().EndsWith("$remove")
                             || key.ToString().Contains("$"))
                    {
                        // Append new value to old list
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
                        if (key.ToString().EndsWith("$add")
                            || key.ToString().Contains("$"))
                        {
                            // Recursive call
                            foreach (string key2 in ed.ValuesTable.Keys)
                            {
                                oldEd.OverwriteOrAdd(key2, ed.GetEntityDataFromValueTable(key2));
                            }

                            valuesTable[key] = oldEd;
                        }
                        else
                        {
                            // Pure replace
                            valuesTable[key] = ed;
                        }
                    }
                    else
                    {
                        // Value and OldValue are the same type, Value is an EntityData, but OldValue isn't.
                        // By all rights, this is unreachable code.
                        throw new Exception("WHAT THE ACTUAL HELL JUST HAPPENED?!?!?");
                    }

                    break;
                case int v when (oldValue is int old && (key.ToString().EndsWith("$plus") || key.ToString().EndsWith("$minus"))):
                    // x + a + b = x + (a+b); x - a - b = x - (a+b)
                    // The same effect is achieved by adding b to a.
                    valuesTable[key] = old + v;
                    break;
                case double v when (oldValue is double old && (key.ToString().EndsWith("$plus") || key.ToString().EndsWith("$minus"))):
                    // x + a + b = x + (a+b); x - a - b = x - (a+b)
                    // The same effect is achieved by adding b to a.
                    valuesTable[key] = old + v;
                    break;
                default:
                    // "Whether [the] modder wants [it] or not, default syntax means replacing, and it should stay that way for the sake of compatibility"
                    // - Chelnoque, untitled one
                    valuesTable[key] = value;
                    break;
            }
        }
    }
}