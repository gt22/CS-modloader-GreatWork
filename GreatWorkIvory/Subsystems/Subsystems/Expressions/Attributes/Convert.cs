using System;

namespace GreatWorkIvory.Expressions
{
    
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class Convert : Attribute
    {
        public Type From { get; }
        public string Using { get; }

        public Convert(Type @from, string @using)
        {
            From = @from;
            Using = @using;
        }

    }
}