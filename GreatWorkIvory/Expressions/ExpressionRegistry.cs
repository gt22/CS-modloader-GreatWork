using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GreatWorkIvory.Utils;



namespace GreatWorkIvory.Expressions
{
    using ExprFunc = Func<ExpressionContext, List<object>, object>;
    public static class ExpressionRegistry
    {
        
        private static readonly Dictionary<string, List<ExprFunc>> Operations =
            new Dictionary<string, List<ExprFunc>>();
        
        public static void RegisterFrom(Assembly a)
        {
            foreach (var type in a.GetTypes())
            {
                foreach (var m in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public |
                                                  BindingFlags.NonPublic))
                {
                    if (m.GetCustomAttribute<ExpressionOp>() is ExpressionOp ex)
                    {
                        Operations.ComputeIfAbsent(
                            ex.Op,
                            _ => new List<ExprFunc>()
                        ).Add(ExpressionEvaluator.MethodToExpr(m));
                    }
                }
            }
        }
        
        public static IEnumerable<ExprFunc> Get(string op)
        {
            return Operations[op];
        }
    }
}