using System;
using System.Collections.Generic;
using Assets.CS.TabletopUI;
using GreatWork.Events.EventTypes;
using GreatWork.Utils;

namespace GreatWork.Patches
{
    public static class RegistryPatch
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(Registry).Method("Register").MakeGenericMethod(typeof(object)),
                HarmonyHolder.Wrap("Prefix")
            );
        }

        private static bool Prefix(object toRegister, Dictionary<Type, object> ___registered)
        {
            if (GreatWorkAPI.Events.FireEvent(new RegistryEvent.PreReg(toRegister)))
                for (var cur = toRegister.GetType(); cur != typeof(object) && cur != null; cur = cur.BaseType)
                    ___registered[cur] = toRegister;
            GreatWorkAPI.Events.FireEvent(new RegistryEvent.PostReg(toRegister));
            return false;
        }
    }
}