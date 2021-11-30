using SecretHistories.Entities;
using SecretHistories.UI;

namespace GreatWorkIvory.Expressions
{
    public class ExpressionContext
    {
        public AspectsInContext Aspects { get; }

        public ExpressionContext(AspectsInContext aspects)
        {
            Aspects = aspects;
        }

        public ExpressionContext(Situation s) : this(Watchman.Get<HornedAxe>().GetAspectsInContext(s.GetAspects(true)))
        {
            
        }
    }
}