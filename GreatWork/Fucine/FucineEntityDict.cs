using System;
using Assets.Core.Fucine;

namespace GreatWork.Fucine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FucineEntityDict : Assets.Core.Fucine.Fucine
    {
        public override AbstractImporter CreateImporterInstance()
        {
            return new EntityDictImporter();
        }
    }
}