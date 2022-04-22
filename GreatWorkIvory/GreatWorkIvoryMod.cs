using System;
using GreatWorkIvory;
using GreatWorkIvory.Expressions.Handlers;
using GreatWorkIvory.Fucine.ExtraActions;
using GreatWorkIvory.Patches;

// ReSharper disable once CheckNamespace
public class greatworkivory
{
    public static void Initialise()
    {
        try
        {
            NoonUtility.Log("Initializing GreatWork");
            
            GreatWorkPatches.PatchAll();
            Subsystems.Init();
            GreatWorkApi.RegisterCurrentAssembly();
        }
        catch (Exception e)
        {
            NoonUtility.Log($"GW exception: {e}");
        }
    }
}
