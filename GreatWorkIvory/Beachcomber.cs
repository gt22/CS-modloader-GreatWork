using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GreatWorkIvory.Entities;
using GreatWorkIvory.Utils;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory
{
    public static class Beachcomber
    {
        private static Dictionary<Type, Dictionary<string, Type>> ExtensionTypes = new Dictionary<Type, Dictionary<string, Type>>();

        private static Dictionary<IEntityWithId, Dictionary<string, IEntityWithId>> Extensions = new Dictionary<IEntityWithId, Dictionary<string, IEntityWithId>>();

        public static void Register<E, T>(string name) 
            where E : IEntityWithId 
            where T : IEntityWithId
        {
            name = name.ToLower();
            var ext = ExtensionTypes.ComputeIfAbsent(typeof(E), e => new Dictionary<string, Type>());
            if (ext.ContainsKey(name))
            {
                Console.WriteLine("Overriding extension " + name); // TODO: Proper log
            }
            ext[name] = typeof(T);
        }
        
        
        public static void Load(IEntityWithId owner, Hashtable data, ContentImportLog log)
        {
            if (!ExtensionTypes.TryGetValue(owner.GetType(), out var extTypes)) return;
            foreach (var ext in data.Keys)
            {
                var extName = ext.ToString().ToLower();
                if (!extTypes.TryGetValue(extName, out var extType)) continue;
                
                var extVal = data[ext];
                object extData;
                if (extType.GetInterfaces().Any(t => ReflectionUtils.IsSubclassOfRawGeneric(typeof(IBeachcomberEntity<>), t) ))
                {
                    var newData = new Hashtable();
                    newData["value"] = extVal;
                    extData = new EntityData("", newData);
                }
                else
                {
                    extData = extVal;
                }
                    
                Extensions.ComputeIfAbsent(
                    owner, s => new Dictionary<string, IEntityWithId>()
                )[extName] = EntityUtils.CreateEntity(extType, extData, log);
            }
        }

        public static T Get<T>(this IEntityWithId owner, string name)
        {
            name = name.ToLower();
            IEntityWithId res;
            if (Extensions.ContainsKey(owner) && Extensions[owner].ContainsKey(name))
            {
                res = Extensions[owner][name];
            }
            else
            {
                return default;
            }
            
            if (res is IBeachcomberEntity<T> bres)
            {
                return bres.Value;
            }
            return (T) res;
        }
    }
}