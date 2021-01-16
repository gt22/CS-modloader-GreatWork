using System.Reflection;
using Assets.Core.Services;
using Assets.CS.TabletopUI;
using GreatWork.Events;
using GreatWork.Events.Links;
using GreatWork.Utils;

namespace GreatWork.Patches
{
    [GwEventHandler]
    public static class LeversPatch
    {
        private static readonly FieldInfo CStorage = typeof(Chronicler).Field("_storage");
        private static readonly FieldInfo CCompendium = typeof(Chronicler).Field("_compendium");

        [SubscribeEvent]
        private static void OnCharReg(RegistryLink<Character>.PostReg e)
        {
            var c = Registry.Get<Chronicler>();
            if (c != null) CStorage.SetValue(c, e.Item);
        }

        [SubscribeEvent]
        private static void OnCompReg(RegistryLink<Compendium>.PostReg e)
        {
            var c = Registry.Get<Chronicler>();
            if (c != null) CCompendium.SetValue(c, e.Item);
        }
    }
}