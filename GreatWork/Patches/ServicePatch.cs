using Assets.CS.TabletopUI;
using Assets.TabletopUi.Scripts.Services;
using GreatWork.Events.EventTypes;
using GreatWork.Utils;

namespace GreatWork.Patches
{
    public static class ServicePatch
    {
        public static void PatchAll()
        {
            Patch<Glory>("Initialise");
            Patch<TabletopManager>("Start");
        }

        private static void Patch<T>(string methodName)
        {
            HarmonyHolder.Harmony.Patch(
                typeof(T).Method(methodName),
                HarmonyHolder.Wrap<T>("Prefix"),
                HarmonyHolder.Wrap<T>("Postfix")
            );
        }

        private static bool Prefix<T>(T __instance)
        {
            GreatWorkAPI.Events.FireEvent(new ServiceInitializationEvent<T>.PreInit(__instance));
            return true;
        }

        private static void Postfix<T>(T __instance)
        {
            GreatWorkAPI.Events.FireEvent(new ServiceInitializationEvent<T>.PostInit(__instance));
        }
    }
}