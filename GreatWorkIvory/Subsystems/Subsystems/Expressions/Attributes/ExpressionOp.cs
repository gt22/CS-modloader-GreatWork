using System;

namespace GreatWorkIvory.Expressions
{
    
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ExpressionOp : Attribute
    {
        public string Op { get; }

        public ExpressionOp(string op)
        {
            Op = op;
        }

        public ExpressionOp(Type t)
        {
            Op = t.Name;
        }
    }
}