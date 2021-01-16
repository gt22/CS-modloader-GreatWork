using Assets.Core.Fucine;
using Assets.Core.Fucine.DataImport;

namespace GreatWork.Fucine
{
    public class SpriteImporter : AbstractImporter
    {
        private readonly string _folder;

        public SpriteImporter(string folder)
        {
            _folder = folder;
        }

        public override bool TryImportProperty<T>(T entity, CachedFucineProperty<T> prop, EntityData entityData,
            ContentImportLog log)
        {
            var name = entityData.ValuesTable[prop.LowerCaseName];
            if (name == null) return false;
            var sprite = ResourcesManager.GetSprite(_folder, name.ToString(), false);
            prop.SetViaFastInvoke(entity, sprite);
            return true;
        }
    }
}