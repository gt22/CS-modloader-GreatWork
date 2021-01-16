using BepInEx;
using GreatWork.Patches;

namespace GreatWork
{
    [BepInPlugin("greatwork", "Great Work", "0.1.0.0")]
    public class GreatWork : BaseUnityPlugin
    {
        private void Awake()
        {
            PropertyPatch.PatchAll();
            RegistryPatch.PatchAll();
            ServicePatch.PatchAll();
            CompendiumPatch.PatchAll();
            GreatWorkAPI.RegisterCurrentAssembly();
        }
    }
}