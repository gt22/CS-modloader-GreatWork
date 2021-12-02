using System.Collections.Generic;
using GreatWorkIvory.Entities;
using GreatWorkIvory.Events;
using GreatWorkIvory.Events.Links;
using SecretHistories.Core;
using SecretHistories.Entities;
using SecretHistories.Enums;

namespace GreatWorkIvory.Expressions.Handlers
{
    [GwEventHandler]
    public class RecipeExpressions
    {

        public const string EffectProp = "expr_effects";
        
        public static void Init()
        {
            Beachcomber.Register<Recipe, DictEntity<ExprEntity>>(EffectProp);
        }

        [SubscribeEvent]
        public static void OnRecipeEffects(SituationLinks.CommandExecuted<RecipeCompletionEffectCommand>.Post e)
        {
            var exprEffects = e.Command.Recipe.Get<Dictionary<string, ExprEntity>>(EffectProp);
            if(exprEffects == null) return;
            
            var resEffects = new Dictionary<string, int>();
            foreach (var effect in exprEffects)
            {
                resEffects[effect.Key] = effect.Value.Eval<int>(new ExpressionContext(e.Situation));
            }

            var sphere = e.Situation.GetSingleSphereByCategory(SphereCategory.SituationStorage);
            foreach (var effect in resEffects)
            {
                sphere.ModifyElementQuantity(effect.Key, effect.Value, new Context(Context.ActionSource.SituationEffect));
            }
        }
    }
}