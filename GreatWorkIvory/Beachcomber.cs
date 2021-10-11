using System;
using System.Collections.Generic;
using System.Linq;
using GreatWorkIvory.Utils;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory
{
    public static class Beachcomber
    {
        private static Dictionary<string, Type> ExtensionTypes = new Dictionary<string, Type>();

        private static Dictionary<IEntityWithId, Dictionary<Type, IEntityWithId>> Extensions = new Dictionary<IEntityWithId, Dictionary<Type, IEntityWithId>>();

        public static void Register<T>(string name) where T : IEntityWithId
        {
            if (ExtensionTypes.ContainsKey(name))
            {
                Console.WriteLine("Overriding extension " + name); // TODO: Proper log
            }
            ExtensionTypes[name] = typeof(T);
        }
        public static void Load(IEntityWithId owner, EntityData data, ContentImportLog log)
        {
            foreach (var ext in data.ValuesTable.Keys)
            {
                var extName = ext.ToString();
                if (ExtensionTypes.TryGetValue(extName, out var extType))
                {
                    Extensions.ComputeIfAbsent(
                        owner, s => new Dictionary<Type, IEntityWithId>()
                    )[extType] = EntityUtils.CreateEntity(extType, data.ValuesTable[ext], log);
                }
            }
        }

        public static T Get<T>(IEntityWithId owner) where T : class, IEntityWithId
        {
            return Extensions[owner][typeof(T)] as T;
        }
    }
}