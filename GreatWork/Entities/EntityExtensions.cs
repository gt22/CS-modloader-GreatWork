using System.Collections;
using Assets.Core.Fucine;

namespace GreatWork.Entities
{
    public static class EntityExtensions
    {
        public static Hashtable Extra<T>(this AbstractEntity<T> p) where T : AbstractEntity<T>
        {
            return p.PopAllUnknownProperties();
        }
    }
}