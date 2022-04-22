namespace GreatWorkIvory.Patches
{
    public class GreatWorkPatches
    {

        public static void PatchAll()
        {
            ServicePatch.PatchAll();
            RegistryPatch.PatchAll();
            BeachcomberPatch.PatchAll();
            CompendiumPatch.PatchAll();
            SituationPatches.PatchAll();
            DictOfEntitiesPatch.PatchAll();
            RecipeConductorPatch.PatchAll();
        }
        
    }
}