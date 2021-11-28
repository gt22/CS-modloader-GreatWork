using System;
using GreatWorkIvory;
using GreatWorkIvory.Patches;
using SecretHistories.Entities;

// ReSharper disable once CheckNamespace
public class GreatWorkIvoryMod
{
    public static void Initialise()
    {
        try
        {
            Console.WriteLine("Great Work Begins");
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
