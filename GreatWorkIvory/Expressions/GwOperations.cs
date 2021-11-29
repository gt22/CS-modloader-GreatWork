using System;
using System.Linq;
using SecretHistories.Entities;
using SecretHistories.UI;

namespace GreatWorkIvory.Expressions
{
    public static class GwOperations
    {
        [ExpressionOp("+")]
        public static int Add(
            [Implicit] int a,
            [Implicit] int b
        )
        {
            return a + b;
        }

        [ExpressionOp("<")]
        public static bool Lt(
            [Implicit] int a,
            [Implicit] int b
        )
        {
            return a < b;
        }
        
        [ExpressionOp(">")]
        public static bool Gt(
            [Implicit] int a,
            [Implicit] int b
        )
        {
            return a > b;
        }
        
        [ExpressionOp("=")]
        public static bool Eq(
            [Implicit] int a,
            [Implicit] int b
        )
        {
            return a == b;
        }

        [ExpressionOp("value")]
        public static int Value(
            string x
        )
        {
            return int.Parse(x);
        }

        private static string СheckAspect(string id)
        {
            if (!Watchman.Get<Compendium>().EntityExists<Element>(id))
            {
                throw new ArgumentException();
            }

            return id;
        }

        [ExpressionOp("aspect")]
        public static int Aspect(ExpressionContext ctx, string id)
        {
            return ctx.aspects._aspectsInSituation.AspectValue(СheckAspect(id));
        }

        [ExpressionOp("table")]
        public static int AspectTable(ExpressionContext ctx, string id)
        {
            return ctx.aspects._aspectsOnTable.AspectValue(СheckAspect(id));
        }

        [ExpressionOp("extant")]
        public static int AspectExtant(ExpressionContext ctx, string id)
        {
            return ctx.aspects._aspectsExtant.AspectValue(СheckAspect(id));
        }

        [ExpressionOp(typeof(bool))]
        public static bool Int2Bool(
            int x
        )
        {
            return x != 0;
        }

        [ExpressionOp(typeof(int))]
        public static int StrToInt(
            [
                Convert(typeof(string), "value"),
                Convert(typeof(string), "aspect")
            ]
            int x
        )
        {
            return x;
        }

        [ExpressionOp(typeof(int))]
        public static int BoolToInt(
            bool x
        )
        {
            return x ? 1 : 0;
        }

        [ExpressionOp("&")]
        public static bool And(
            [Implicit] params bool[] a
        )
        {
            return a.All(x => x);
        }
    }
}