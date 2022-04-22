using GreatWorkIvory.Events.EventTypes;
using GreatWorkIvory.Utils;
using SecretHistories.Services;
using SecretHistories.UI;

namespace GreatWorkIvory.Patches
{
    public static class ServicePatch
    {
        public static void PatchAll()
        {
            Patch<MenuScreenController>("InitialiseServices");
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
            GreatWorkApi.Events.FireEvent(new ServiceInitializationEvent<T>.PreInit(__instance));
            return true;
        }

        private static void Postfix<T>(T __instance)
        {
            GreatWorkApi.Events.FireEvent(new ServiceInitializationEvent<T>.PostInit(__instance));
        }
    }
}