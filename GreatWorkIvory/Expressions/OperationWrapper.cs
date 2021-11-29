using System;
using System.Linq.Expressions;

namespace GreatWorkIvory.Expressions
{
    public class OperationWrapper
    {
        
        // public static Func<ExpressionContext, EntityExpr, object> WrapOp<A>(Func<A, int> f)
        // {
        //     return (ctx, ex) =>
        //     {
        //         if (ExpressionRegistry.Eval(ctx, ex.Operands[0]) is A a)
        //         {
        //             return f(a);
        //         }
        //
        //         throw new ArgumentException(ex.ToString());
        //     };
        // }
        // public static Func<ExpressionContext, EntityExpr, object> WrapOp<A, B>(Func<A, B, int> f)
        // {
        //     return (ctx, ex) =>
        //     {
        //         if (ExpressionRegistry.Eval(ctx, ex.Operands[0]) is A a)
        //         {
        //             if (ExpressionRegistry.Eval(ctx, ex.Operands[1]) is B b)
        //             {
        //                 return f(a, b);
        //             }
        //         }
        //         throw new ArgumentException(ex.ToString());
        //     };
        // }

        public static Func<ExpressionContext, EntityExpr, object> WrapOp(LambdaExpression f)
        {
            return (ctx, ex) =>
            {
                for (var i = 0; i < f.Parameters.Count; i++)
                {
                    
                }

                throw new ArgumentException(ex.ToString());
            };
        }
    }
}