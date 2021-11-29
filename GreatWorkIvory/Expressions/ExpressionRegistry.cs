using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GreatWorkIvory.Utils;

namespace GreatWorkIvory.Expressions
{
    public static class ExpressionRegistry
    {
        private static readonly Dictionary<string, List<Func<ExpressionContext, object[], object>>> Operations =
            new Dictionary<string, List<Func<ExpressionContext, object[], object>>>();

        private static bool ApplyExplicitConverts(ExpressionContext ctx, ParameterInfo p, object r, out object res)
        {
            var conv = p.GetCustomAttributes<Convert>();
            foreach (var convert in conv)
            {
                if (convert.From.IsInstanceOfType(r))
                {
                    try
                    {
                        res = Eval(ctx, convert.Using, new[] {r});
                        return true;
                    }
                    catch (ArgumentException)
                    {
                        
                    }
                }
            }
            res = r;
            return false;
        }

        private static bool ApplyImplicitConvert(ExpressionContext ctx, ParameterInfo p, object r, out object res)
        {
            try
            {
                if (p.GetCustomAttribute<Implicit>() != null)
                {
                    res = Eval(ctx, p.ParameterType.Name, new[] {r});
                    return true;
                }
            }
            catch (ArgumentException)
            {
                
            }
            res = r;
            return false;
        }
        
        private static object TryConvert(ExpressionContext ctx, ParameterInfo p, object r)
        {
            if (p.ParameterType.IsInstanceOfType(r)) return r;
            if (ApplyExplicitConverts(ctx, p, r, out var conv)) return conv;
            if (ApplyImplicitConvert(ctx, p, r, out conv)) return conv;
            throw new ArgumentException();
        }
        
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
                            _ => new List<Func<ExpressionContext, object[], object>>()
                        ).Add(
                            (ctx, val) =>
                            {
                                var res = new List<object>();
                                var i = 0;
                                foreach (var p in m.GetParameters())
                                {
                                    Console.WriteLine(p.Attributes);
                                    if (p.ParameterType == typeof(ExpressionContext))
                                    {
                                        res.Add(ctx);
                                    }
                                    else if (p.GetCustomAttribute<ParamArrayAttribute>() != null)
                                    {
                                        Console.WriteLine(p);
                                    }
                                    else
                                    {
                                        var r = val[i++];
                                        res.Add(TryConvert(ctx, p, r));
                                    }
                                }

                                // return m.Invoke(null, res.ToArray()); 
                                return 1;
                            }
                        );
                    }
                }
            }
        }

        public static object Eval(ExpressionContext ctx, string op, object[] values)
        {
            if (op.StartsWith("$"))
                return op.Substring(1);
            foreach (var func in Operations[op])
            {
                try
                {
                    return func(ctx, values);
                }
                catch (ArgumentException e)
                {
                }
            }

            throw new ArgumentException();
        }

        public static object Eval(ExpressionContext ctx, EntityExpr ex)
        {
            var values = ex.Operands.Select(e => Eval(ctx, e)).ToArray();
            try
            {
                return Eval(ctx, ex.Op, values);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException(ex.ToString());
            }
        }
    }
}