namespace GreatWorkIvory.Expressions.Operations
{
    public static class TypeConverters
    {
        [ExpressionOp(typeof(int))]
        public static int Str2Int(
            [
                Convert(typeof(string), "value"),
                Convert(typeof(string), "aspect")
            ]
            int x
        ) => x;

        [ExpressionOp(typeof(int))]
        public static int BoolToInt(bool x) => x ? 1 : 0;

        [ExpressionOp(typeof(bool))]
        public static bool Int2Bool(int x) => x != 0;

        [ExpressionOp(typeof(double))]
        public static double IntToDouble(int x) => x;

        [ExpressionOp(typeof(int))]
        public static int Trunc(double x) => (int) x;

        [ExpressionOp(typeof(double))]
        public static double DoubleLiteral(
            [Convert(typeof(string), "valueDouble")]
            double x
        ) => x;

        [ExpressionOp(typeof(object))]
        public static object OjbectID(object x) => x;
    }
}