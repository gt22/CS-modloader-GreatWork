using System;
using GreatWorkIvory;
using GreatWorkIvory.Entities;
using GreatWorkIvory.Expressions;
using GreatWorkIvory.Expressions.Handlers;
using GreatWorkIvory.Patches;
using SecretHistories.Entities;

// ReSharper disable once CheckNamespace
public class greatworkivory
{
    public static void Initialise()
    {
        try
        {
            NoonUtility.Log("Initializing GreatWork");
            ServicePatch.PatchAll();
            RegistryPatch.PatchAll();
            BeachcomberPatch.PatchAll();
            CompendiumPatch.PatchAll();
            SituationPatches.PatchAll();
            DictOfEntitiesPatch.PatchAll();
            
            ExprHandlers.Init();
            Madrugad.Init();
            GreatWorkApi.RegisterCurrentAssembly();
            
        }
        catch (Exception e)
        {
            NoonUtility.Log($"GW exception: {e}");
        }
    }
}
