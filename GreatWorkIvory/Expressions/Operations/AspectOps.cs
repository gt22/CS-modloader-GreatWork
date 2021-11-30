using System;
using SecretHistories.Entities;
using SecretHistories.UI;

namespace GreatWorkIvory.Expressions.Operations
{
    public static class AspectOps
    {
        private static string IsAspect(string id)
        {
            if (!Watchman.Get<Compendium>().EntityExists<Element>(id))
            {
                throw new ArgumentException();
            }

            return id;
        }

        [ExpressionOp("aspect")]
        public static int Aspect(ExpressionContext ctx, string id) => 
            ctx.Aspects._aspectsInSituation.AspectValue(IsAspect(id));

        [ExpressionOp("table")]
        public static int AspectTable(ExpressionContext ctx, string id) => 
            ctx.Aspects._aspectsOnTable.AspectValue(IsAspect(id));

        [ExpressionOp("extant")]
        public static int AspectExtant(ExpressionContext ctx, string id) => 
            ctx.Aspects._aspectsExtant.AspectValue(IsAspect(id));
    }
}