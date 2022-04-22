using GreatWorkIvory.Expressions.Handlers;
using GreatWorkIvory.Fucine.ExtraActions;

namespace GreatWorkIvory
{
    public class Subsystems
    {
        public static void Init()
        {
            ExprHandlers.Init();
            AdvancedRecipeLinks.Init();
            Madrugad.Init();
        }
    }
}