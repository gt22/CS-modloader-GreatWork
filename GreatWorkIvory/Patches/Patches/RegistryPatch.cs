using System;
using System.Collections.Generic;
using GreatWorkIvory.Events.EventTypes;
using GreatWorkIvory.Utils;
using SecretHistories.UI;

namespace GreatWorkIvory.Patches
{
    public static class RegistryPatch
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(Watchman).Method("Register").MakeGenericMethod(typeof(object)),
                HarmonyHolder.Wrap("Prefix")
            );
        }

        private static bool Prefix(object toRegister, Dictionary<Type, object> ___registered)
        {
            if (toRegister == null) {
                return true;
            }
            if (GreatWorkApi.Events.FireEvent(new RegistryEvent.PreReg(toRegister)))
                for (var cur = toRegister.GetType(); cur != typeof(object) && cur != null; cur = cur.BaseType)
                    ___registered[cur] = toRegister;
            GreatWorkApi.Events.FireEvent(new RegistryEvent.PostReg(toRegister));
            return false;
        }
    }
}