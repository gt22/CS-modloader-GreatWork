using System;
using Roost;
using Roost.Vagabond;
using SecretHistories.Core;
using SecretHistories.Entities;
using SecretHistories.UI;

namespace GreatWorkIvory.Expressions.Handlers
{
    public class ExprHandlers
    {
        public static void Init()
        {
            RecipeExpressions.Init();
            CommandLine.AddCommand("expr", args =>
            {
                ExprParser.Parse(string.Join(" ", args)).IfSome(expr =>
                {
                    Birdsong.Sing(
                        $"{expr} = {expr.Eval<object>(new ExpressionContext(Watchman.Get<HornedAxe>().GetAspectsInContext(new AspectsDictionary(), null)))}");
                });
            });
        }
    }
}