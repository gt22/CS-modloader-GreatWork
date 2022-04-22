using System.Collections.Generic;
using System.Linq;
using GreatWorkIvory.Entities;
using GreatWorkIvory.Events;
using GreatWorkIvory.Events.EventTypes;
using Roost;
using SecretHistories.Core;
using SecretHistories.Entities;
using SecretHistories.UI;

namespace GreatWorkIvory.Fucine.ExtraActions
{
    [GwEventHandler]
    public class AdvancedRecipeLinks
    {
        public const string EqualChance = "linked_equal_chance";

        public static void Init()
        {
            Beachcomber.Register<Recipe, ListEntity<LinkedRecipeDetails>>(EqualChance);
        }

        public static Recipe SelectLink(RecipeConductor c, List<LinkedRecipeDetails> links)
        {
            var tmpRecipe = Recipe.CreateSpontaneousHintRecipe(Verb.CreateSpontaneousVerb("tmp", "tmp", "tmp"));
            tmpRecipe.SetId("GreatWork.AdvancedRecipeLinks#SelectLink");
            tmpRecipe.Linked = links;
            return c.GetLinkedRecipe(tmpRecipe);
        }

        public static Recipe SelectLink(AspectsInContext aspects, List<LinkedRecipeDetails> links) =>
            SelectLink(new RecipeConductor(aspects, Watchman.Get<Stable>().Protag()), links);

        [SubscribeEvent]
        public static void OnRecipeLink(RecipeLinkEvent e)
        {
            var l = e.Source.Get<List<LinkedRecipeDetails>>(EqualChance);
            if (l == null) return;
            int passes = 0;
            foreach (var r in l.AsQueryable().Reverse())
            {
                r.Chance = Watchman.Get<Compendium>().GetEntityById<Recipe>(r.Id).RequirementsSatisfiedBy(e.Aspects)
                    ? 100 / ++passes
                    : 0;
            }

            if (passes > 0)
            {
                e.Linked = SelectLink(e.Conductor, l);
            }
        }
    }
}