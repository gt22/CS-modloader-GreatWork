using System;
using GreatWorkIvory;
using GreatWorkIvory.Entities;
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
            
            GreatWorkApi.RegisterCurrentAssembly();
            
            Beachcomber.Register<Recipe, ExprEntity>("expr_test");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
