using System.Linq;

namespace GreatWorkIvory.Expressions.Operations
{
    public class Logic
    {
        [ExpressionOp("<")]
        public static bool Lt([Implicit] int a, [Implicit] int b) => a < b;

        [ExpressionOp(">")]
        public static bool Gt([Implicit] int a, [Implicit] int b) => a > b;
        
        [ExpressionOp("<=")]
        public static bool LtEq([Implicit] int a, [Implicit] int b) => a <= b;

        [ExpressionOp(">=")]
        public static bool GtEq([Implicit] int a, [Implicit] int b) => a >= b;

        [ExpressionOp("=")]
        public static bool Eq(params object[] x)
        {
            if (x.Length == 0) return true;
            var b = x[0];
            return x.All(a => a.Equals(b));
        }

        [ExpressionOp("&")]
        public static bool And([Implicit] params bool[] a) => a.All(x => x);

        [ExpressionOp("|")]
        public static bool Or([Implicit] params bool[] a) => a.Any();
    }
}