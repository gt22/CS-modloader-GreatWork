using System;
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
            GreatWorkAPI.ReloadCompendium();
        }


        [SubscribeEvent]
        public static void CompendiumLoad(CompendiumEvent.End e)
        {
            try
            {
                e.Compendium.GetEntitiesAsList<EntityExpr>().ForEach(ex => NoonUtility.Log(ex + $" - {ExpressionRegistry.Eval(null, ex)}"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }
        
        [SubscribeEvent]
        public static void GameStart(RegistryLink<Numa>.PostReg e)
        {
            
        }
    }
}