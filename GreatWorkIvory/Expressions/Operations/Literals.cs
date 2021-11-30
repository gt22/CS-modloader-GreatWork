using System.IO;

namespace GreatWorkIvory.Expressions.Operations
{
    public static class Literals
    {
        [ExpressionOp("value")]
        public static int Value(string x) => int.Parse(x);

        [ExpressionOp("valueDouble")]
        public static double ValueDouble(string x) => double.Parse(x);

        [ExpressionOp("value")]
        public static int Value(int x) => x;

        [ExpressionOp("str")]
        public static string Str(object x) => x.ToString();
    }
}