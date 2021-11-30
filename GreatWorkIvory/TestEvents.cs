using System;
using GreatWorkIvory.Entities;
using GreatWorkIvory.Events;
using GreatWorkIvory.Events.EventTypes;
using GreatWorkIvory.Events.Links;
using GreatWorkIvory.Expressions;
using SecretHistories.Entities;
using SecretHistories.Services;
using SecretHistories.UI;

namespace GreatWorkIvory
{
    [GwEventHandler]
    public class TestEvents
    {
        [SubscribeEvent]
        public static void GloryLoad(RegistryLink<ScreenResolutionAdapter>.PostReg e)
        {
            // GreatWorkAPI.ReloadCompendium();
        }


        [SubscribeEvent]
        public static void CompendiumLoad(CompendiumEvent.End e)
        {

        }
        
        [SubscribeEvent]
        public static void GameStart(RegistryLink<Numa>.PostReg e)
        {
            
        }
    }
}