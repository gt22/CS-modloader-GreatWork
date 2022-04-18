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
            try
            {
                foreach (var ex in e.Compendium.GetEntitiesAsList<Recipe>().Map(r => r.Get<ExprEntity>("expr_test")).Filter(x => x != null))
                {
                    NoonUtility.Log(ex + $" - {ex.Eval(null)} - Recipe");
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }
        
        [SubscribeEvent]
        public static void GameStart(RegistryLink<Numa>.PostReg e)
        {
            NoonUtility.Log("Game Start");
            Madrugad.SetFlag("test", 1);
        }
    }
}