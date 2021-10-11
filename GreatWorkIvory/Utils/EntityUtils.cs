using System;
using System.Linq;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Utils
{
    public static class EntityUtils
    {
        public static IEntityWithId CreateEntity(Type entityType, object data, ContentImportLog log)
        {
            IEntityWithId res;
            if (data is string quickspec)
            {
                if (FactoryInstantiator.CreateObjectWithDefaultConstructor(entityType) is IQuickSpecEntity ent)
                {
                    ent.QuickSpec(quickspec);
                    res = ent as IEntityWithId;
                }
                else
                {
                    throw new ApplicationException("Expected " + entityType + " to be IQuickSpecEntity");
                }
            }
            else
            {
                res = FactoryInstantiator.CreateEntity(entityType, data as EntityData, log);
            }

            return res;
        }
    }
}