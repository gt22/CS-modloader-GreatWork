using System.Collections;
using System.Linq;
using Assets.Core.Fucine;
using Assets.Core.Fucine.DataImport;
using Assets.Core.Interfaces;

namespace GreatWork.Fucine
{
    public class EntityDictImporter : AbstractImporter
    {
        public override bool TryImportProperty<T>(
            T entity,
            CachedFucineProperty<T> prop,
            EntityData entityData,
            ContentImportLog log)
        {
            var propertyType = prop.ThisPropInfo.PropertyType;
            if (!(entityData.ValuesTable[prop.LowerCaseName] is EntityData subEntityData))
            {
                prop.SetViaFastInvoke(entity, FactoryInstantiator.CreateObjectWithDefaultConstructor(propertyType));
                return false;
            }

            var entityType = propertyType.GetGenericArguments()[1];
            if (!(FactoryInstantiator.CreateObjectWithDefaultConstructor(propertyType) is IDictionary target))
                return false;
            foreach (var k in subEntityData.ValuesTable.Keys)
            {
                var kk = k.ToString().ToLower();
                var data = subEntityData.ValuesTable[kk];
                if (entityType.GetInterfaces().Contains(typeof(IQuickSpecEntity)) &&
                    data is string qs)
                {
                    if (!(FactoryInstantiator.CreateObjectWithDefaultConstructor(entityType) is IQuickSpecEntity
                        defaultConstructor))
                        return false;
                    defaultConstructor.QuickSpec(qs);
                    target[kk] = defaultConstructor;
                }
                else
                {
                    var ent = FactoryInstantiator.CreateEntity(entityType, data as EntityData, log);
                    target[kk] = ent;
                }
            }

            prop.SetViaFastInvoke(entity, target);
            return true;
        }
    }
}