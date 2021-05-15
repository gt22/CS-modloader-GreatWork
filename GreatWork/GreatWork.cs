using System.IO;
using BepInEx;
using GreatWork.Patches;
using UnityEngine;

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
            ModPatch.PatchAll();
            ResourcePatch.PatchAll();
            RefinementPatch.PatchAll();
            OverwriteOrAddPatch.PatchAll();
            GreatWorkAPI.RegisterCurrentAssembly();
        }
    }
}