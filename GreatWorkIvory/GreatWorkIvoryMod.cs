using System;
using GreatWorkIvory;
using GreatWorkIvory.Expressions;
using GreatWorkIvory.Patches;
using SecretHistories.Entities;

// ReSharper disable once CheckNamespace
public class GreatWorkIvoryMod
{
    public static void Initialise()
    {
        try
        {
            ServicePatch.PatchAll();
            RegistryPatch.PatchAll();
            BeachcomberPatch.PatchAll();
            CompendiumPatch.PatchAll();
            
            GreatWorkAPI.RegisterCurrentAssembly();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
