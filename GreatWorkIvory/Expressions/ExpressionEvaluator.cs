using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GreatWorkIvory.Entities;

namespace GreatWorkIvory.Expressions
{
    using ExprFunc = Func<ExpressionContext, List<object>, object>;
    public static class ExpressionEvaluator
    {
        
        private static bool ApplyExplicitConverts(ExpressionContext ctx, ParameterInfo p, Type target, object r,
            out object res)
        {
            var conv = p.GetCustomAttributes<Convert>();
            foreach (var convert in conv)
            {
                if (!convert.From.IsInstanceOfType(r)) continue;
                try
                {
                    res = TryConvert(ctx, p, target, Eval(ctx, convert.Using, new List<object> {r}));
                    return true;
                }
                catch (ArgumentException)
                {
                }
            }

            res = r;
            return false;
        }

        private static bool ApplyImplicitConvert(ExpressionContext ctx, ParameterInfo p, Type target, object r,
            out object res)
        {
            try
            {
                if (p.GetCustomAttribute<Implicit>() != null)
                {
                    res = TryConvert(ctx, p, target, Eval(ctx, target.Name, new List<object> {r}));
                    return true;
                }
            }
            catch (ArgumentException)
            {
            }

            res = r;
            return false;
        }

        private static object TryConvert(ExpressionContext ctx, ParameterInfo p, Type target, object r)
        {
            if (target.IsInstanceOfType(r)) return r;
            if (ApplyExplicitConverts(ctx, p, target, r, out var conv)) return conv;
            if (ApplyImplicitConvert(ctx, p, target, r, out conv)) return conv;
            throw new ArgumentException();
        }

        private static object BuildParamArray(ExpressionContext ctx, ParameterInfo p, List<object> val, int i)
        {
            var t = p.ParameterType.GetElementType() ?? throw new InvalidOperationException();
            var paramsArray = Array.CreateInstance(t, val.Count - i);
            for (var j = 0; j < paramsArray.Length; j++)
            {
                paramsArray.SetValue(TryConvert(ctx, p, t, val[i++]), j);
            }

            return paramsArray;
        }

        private static object AddParam(ExpressionContext ctx, ParameterInfo p, List<object> val, int i) =>
            p.GetCustomAttribute<ParamArrayAttribute>() != null
                ? BuildParamArray(ctx, p, val, i)
                : TryConvert(ctx, p, p.ParameterType, val[i]);

        private static bool IsCtxParam(ParameterInfo p) => p.ParameterType == typeof(ExpressionContext);

        public static ExprFunc MethodToExpr(MethodInfo m) => (ctx, val) =>
        {
            var i = 0;
            return m.Invoke(null,
                m.GetParameters().Select(
                    p => IsCtxParam(p) ? ctx : AddParam(ctx, p, val, i++)
                ).ToArray()
            );
        };
        
        public static object Eval(ExpressionContext ctx, string op, List<object> values)
        {
            if (op.StartsWith("$"))
                return op.Substring(1);
            foreach (var func in ExpressionRegistry.Get(op))
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

        public static object Eval(this ExprEntity ex, ExpressionContext ctx)
        {
            var values = ex.Operands.Select(e => e.Eval(ctx)).ToList();
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