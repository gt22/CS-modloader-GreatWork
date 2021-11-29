using SecretHistories.Entities;

namespace GreatWorkIvory.Expressions
{
    public class ExpressionContext
    {
        public AspectsInContext aspects { get; }

        public ExpressionContext(AspectsInContext aspects)
        {
            this.aspects = aspects;
        }
    }
}