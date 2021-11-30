using SecretHistories.Entities;

namespace GreatWorkIvory.Expressions
{
    public class ExpressionContext
    {
        public AspectsInContext Aspects { get; }

        public ExpressionContext(AspectsInContext aspects)
        {
            Aspects = aspects;
        }
    }
}