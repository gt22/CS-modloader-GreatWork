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
        private static readonly Dictionary<Type, Dictionary<string, Type>> ExtensionTypes = new Dictionary<Type, Dictionary<string, Type>>();

        private static readonly Dictionary<IEntityWithId, Dictionary<string, IEntityWithId>> Extensions = new Dictionary<IEntityWithId, Dictionary<string, IEntityWithId>>();

        public static void Register<E, T>(string name) 
            where E : IEntityWithId 
            where T : IEntityWithId
        {
            name = name.ToLower();
            var ext = ExtensionTypes.ComputeIfAbsent(typeof(E), e => new Dictionary<string, Type>());
            if (ext.ContainsKey(name))
            {
                NoonUtility.Log($"Overriding extension {name}");
            }
            ext[name] = typeof(T);
        }

        private static bool IsBeachcomberEntity(this Type t) => 
            t.GetInterfaces().Any(i => ReflectionUtils.IsSubclassOfRawGeneric(typeof(IBeachcomberEntity<>), i));
        
        public static void Load(IEntityWithId owner, Hashtable data, ContentImportLog log)
        {
            if (!ExtensionTypes.TryGetValue(owner.GetType(), out var extTypes)) return;
            foreach (var ext in data.Keys)
            {
                var extName = ext.ToString().ToLower();
                if (!extTypes.TryGetValue(extName, out var extType)) continue;
                
                var extVal = data[ext];
                var extData = 
                    extType.IsBeachcomberEntity() 
                    ? new EntityData("", new Hashtable {["value"] = extVal}) 
                    : extVal;
                    
                Extensions.ComputeIfAbsent(
                    owner, s => new Dictionary<string, IEntityWithId>()
                )[extName] = EntityUtils.CreateEntity(extType, extData, log);
            }
        }

        public static T Get<T>(this IEntityWithId owner, string name)
        {
            name = name.ToLower();
            switch (Extensions.GetOrNull(owner)?.GetOrNull(name))
            {
                case null:
                    return default;
                case IBeachcomberEntity<T> bres:
                    return bres.Value;
                case T r:
                    return r;
                default:
                    throw new InvalidCastException(name);
            }
        }
    }
}