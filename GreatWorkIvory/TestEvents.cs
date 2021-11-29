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
                foreach (var ex in e.Compendium.GetEntitiesAsList<Recipe>().Map(r => r.Get<EntityExpr>("expr_test")).Filter(x => x != null))
                {
                    NoonUtility.Log(ex + $" - {ex.Eval(null)} - Recipe");
                }

                e.Compendium.GetEntitiesAsList<EntityExpr>().ForEach(ex => NoonUtility.Log(ex + $" - {ex.Eval(null)}"));
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