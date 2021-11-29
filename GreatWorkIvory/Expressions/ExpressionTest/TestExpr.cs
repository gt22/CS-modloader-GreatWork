using System;
using System.Collections.Generic;

namespace GreatWorkIvory.Expressions.ExpressionTest
{
    public static class TestExpr
    {

        public static void Test()
        {
            var src = "+[$4, extant[$lantern]]";
            //Console.WriteLine(ExprParser.Parse(src));
            Console.WriteLine(new EntityExpr("+", new List<EntityExpr>() { new EntityExpr("$4"), new EntityExpr("$5") }));
        }

        public static void Main(string[] args)
        {
            Test();
        }

    }
}