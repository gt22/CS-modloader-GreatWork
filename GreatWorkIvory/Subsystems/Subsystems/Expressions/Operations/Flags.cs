
namespace GreatWorkIvory.Expressions.Operations
{
    public class Flags
    {

        [ExpressionOp("flag")]
        public static int GetFlag([Implicit] string name)
        {
            return Madrugad.GetFlag(name);
        }

        [ExpressionOp("rawFlag")]
        public static int GetRawFlag([Implicit] string name)
        {
            return Madrugad.GetRawFlag(name);
        }
        
    }
}