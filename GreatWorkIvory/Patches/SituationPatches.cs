using System;
using GreatWorkIvory.Events.EventTypes;
using GreatWorkIvory.Utils;
using SecretHistories.Commands;
using SecretHistories.Entities;
using SecretHistories.Fucine;

namespace GreatWorkIvory.Patches
{
    public static class SituationPatches
    {
        public static void PatchAll()
        {
            HarmonyHolder.Harmony.Patch(
                typeof(Situation).Constructor(typeof(Verb), typeof(string)),
                postfix: HarmonyHolder.Wrap("OnSituationConstruction")
            );
            foreach (var command in typeof(ISituationCommand).Assembly.GetTypes())
            {
                if (typeof(ISituationCommand).IsAssignableFrom(command) && !command.IsInterface)
                {
                    HarmonyHolder.Harmony.Patch(
                        command.Method("Execute", typeof(Situation)),
                        prefix: HarmonyHolder.Wrap("OnCommandExecutedPre"),
                        postfix: HarmonyHolder.Wrap("OnCommandExecutedPost")
                    );
                }
            }
        }

        public static void OnSituationConstruction(Situation __instance)
        {
            __instance.AddSubscriber(SituationEventDispatcher._instance);
        }
        
        public static void OnCommandExecutedPre(ISituationCommand __instance, Situation situation)
        {
            GreatWorkApi.Events.FireEvent(new SituationEvent.CommandExecuted.Pre(situation, __instance));
        }

        public static void OnCommandExecutedPost(ISituationCommand __instance, Situation situation)
        {
            GreatWorkApi.Events.FireEvent(new SituationEvent.CommandExecuted.Post(situation, __instance));
        }

        private class SituationEventDispatcher : ISituationSubscriber
        {
            internal static SituationEventDispatcher _instance = new SituationEventDispatcher();
            
            public void SituationStateChanged(Situation s)
            {
                GreatWorkApi.Events.FireEvent(new SituationEvent.StateChanged(s));
            }

            public void TimerValuesChanged(Situation s)
            {
                GreatWorkApi.Events.FireEvent(new SituationEvent.TimerValueChanged(s));
            }

            public void SituationSphereContentsUpdated(Situation s)
            {
                GreatWorkApi.Events.FireEvent(new SituationEvent.SphereContentChanged(s));
            }
        }
    }
}