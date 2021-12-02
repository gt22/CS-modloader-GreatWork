using System;
using System.Collections;
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
            else if (data is Hashtable ht)
            {
                res = FactoryInstantiator.CreateEntity(entityType, new EntityData(ht), log);
            }
            else
            {
                res = FactoryInstantiator.CreateEntity(entityType, data as EntityData, log);
            }

            return res;
        }
    }
}