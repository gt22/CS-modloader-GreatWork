using System;

namespace GreatWorkIvory.Expressions.Operations
{
    public class Arithmetic
    {
        [ExpressionOp("+")]
        public static int Add([Implicit] int a, [Implicit] int b) => a + b;
        
        [ExpressionOp("-")]
        public static int Sub([Implicit] int a, [Implicit] int b) => a - b;
        
        [ExpressionOp("*")]
        public static int Mul([Implicit] int a, [Implicit] int b) => a * b;
        
        [ExpressionOp("/")]
        public static int FloorDiv([Implicit] int a, [Implicit] int b) => a / b;

        [ExpressionOp("%")]
        public static int Mod([Implicit] int a, [Implicit] int b) => a % b;

        [ExpressionOp("abs")]
        public static int Abs([Implicit] int a) => Math.Abs(a);
        
        [ExpressionOp("^")]
        public static int Pow([Implicit] int x, [Implicit] int p) => (int) Math.Pow(x, p);
        
        [ExpressionOp("f+")]
        public static double Add([Implicit] double a, [Implicit] double b) => a + b;
        
        [ExpressionOp("f-")]
        public static double Sub([Implicit] double a, [Implicit] double b) => a - b;
        
        [ExpressionOp("f*")]
        public static double Mul([Implicit] double a, [Implicit] double b) => a * b;
        
        [ExpressionOp("f/")]
        public static double Div([Implicit] double a, [Implicit] double b) => a / b;
        
        [ExpressionOp("f%")]
        public static double Mod([Implicit] double a, [Implicit] double b) => a % b;
        
        [ExpressionOp("fabs")]
        public static double Abs([Implicit] double a) => Math.Abs(a);

        [ExpressionOp("f^")]
        public static double Pow([Implicit] double x, [Implicit] double p) => Math.Pow(x, p);
    }
}