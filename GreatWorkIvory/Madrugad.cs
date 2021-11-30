using System;
using System.Collections.Generic;
using System.ComponentModel;
using GreatWorkIvory.Entities;
using GreatWorkIvory.Events;
using GreatWorkIvory.Events.EventTypes;
using GreatWorkIvory.Events.Links;
using GreatWorkIvory.Expressions;
using HarmonyLib;
using SecretHistories.Assets.Scripts.Application.Entities.NullEntities;
using SecretHistories.Core;
using SecretHistories.Entities;
using SecretHistories.UI;

namespace GreatWorkIvory
{
    [GwEventHandler]
    public static class Madrugad
    {

        private const string PREFIX = "GreatWork_Flag_";

        private static string Mangle(string name)
        {
            return $"{PREFIX}{name}";
        }
        public static void SetRawFlag(string name, int value, bool additive = false)
        {
            Console.WriteLine($"{name} = {value}");
            FucineRoot.Get().SetMutation(name, value, additive);
        }

        public static int GetRawFlag(string name)
        {
            return FucineRoot.Get().Mutations.GetValueSafe(name);
        }

        public static void SetFlag(string name, int value, bool additive = false)
        {
            SetRawFlag(Mangle(name), value, additive);
        }

        public static int GetFlag(string name)
        {
            return GetRawFlag(Mangle(name));
        }

        internal static void Init()
        {
            Beachcomber.Register<Recipe, DictEntity<List<ExprEntity>>>("set_flags");
        }

        [SubscribeEvent]
        public static void OnRecipeCompletion(SituationLinks.CommandExecuted<RecipeCompletionEffectCommand>.Post e)
        {
            var ops = e.Command.Recipe.Get<Dictionary<string, List<ExprEntity>>>("set_flags");
            Beachcomber.Log(e.Command.Recipe);
            Beachcomber.Log(Watchman.Get<Compendium>().GetEntityById<Recipe>(e.Command.Recipe.Id));
            if(ops == null) return;
            foreach (var op in ops)
            {
                SetFlag(op.Key, op.Value[0].Eval<int>(new ExpressionContext(e.Situation)));
            }
        }
    }
}