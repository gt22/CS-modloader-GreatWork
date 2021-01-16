using System;
using Assets.Core.Fucine;

namespace GreatWork.Fucine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FucineSprite : Assets.Core.Fucine.Fucine
    {
        private readonly string _folder;

        public FucineSprite(string folder)
        {
            _folder = folder;
        }

        public override AbstractImporter CreateImporterInstance()
        {
            return new SpriteImporter(_folder);
        }
    }
}