using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GreatWorkIvory.Utils;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Fucine
{
    public class BetterDictImporter : AbstractImporter
    {
        public override bool TryImportProperty<T>(
            T entity,
            CachedFucineProperty<T> cachedFucinePropertyToPopulate,
            EntityData entityData,
            ContentImportLog log)
        {
            if (!(entityData.ValuesTable[cachedFucinePropertyToPopulate.LowerCaseName] is EntityData
                subEntityData))
            {
                var propertyType = cachedFucinePropertyToPopulate.ThisPropInfo.PropertyType;
                cachedFucinePropertyToPopulate.SetViaFastInvoke(entity,
                    FactoryInstantiator.CreateObjectWithDefaultConstructor(propertyType));
                return false;
            }

            var fucineAttribute = cachedFucinePropertyToPopulate.FucineAttribute as FucineDict;
            var propertiesForType = TypeInfoCache<T>.GetCachedFucinePropertiesForType();
            var propertyType1 = cachedFucinePropertyToPopulate.ThisPropInfo.PropertyType;
            var genericArgument = propertyType1.GetGenericArguments()[1];
            var defaultConstructor =
                FactoryInstantiator.CreateObjectWithDefaultConstructor(propertyType1) as IDictionary;
            if (genericArgument == typeof(string))
                PopulateAsDictionaryOfStrings(entity, cachedFucinePropertyToPopulate, subEntityData,
                    defaultConstructor, log);
            else if (genericArgument == typeof(int))
                PopulateAsDictionaryOfInts(entity, cachedFucinePropertyToPopulate, subEntityData,
                    defaultConstructor, log);
            else if (genericArgument.IsGenericType && genericArgument.GetGenericTypeDefinition() == typeof(List<>))
                PopulateAsDictionaryOfLists(entity, cachedFucinePropertyToPopulate, genericArgument,
                    subEntityData, defaultConstructor, log);
            else
                PopulateAsDictionaryOfEntities(entity, cachedFucinePropertyToPopulate, subEntityData,
                    genericArgument, defaultConstructor, log);
            if (fucineAttribute != null && defaultConstructor != null)
                ValidateKeysMustExistIn(entity, cachedFucinePropertyToPopulate, fucineAttribute.KeyMustExistIn,
                    propertiesForType, defaultConstructor.Keys, log);
            return true;
        }

        public void PopulateAsDictionaryOfStrings<T>(
            T entity,
            CachedFucineProperty<T> cachedFucinePropertyToPopulate,
            EntityData subEntityData,
            IDictionary dictionary,
            ContentImportLog log)
            where T : AbstractEntity<T>
        {
            foreach (DictionaryEntry dictionaryEntry in subEntityData.ValuesTable)
                dictionary.Add(dictionaryEntry.Key, dictionaryEntry.Value.ToString());
            cachedFucinePropertyToPopulate.SetViaFastInvoke(entity, dictionary);
        }

        public void PopulateAsDictionaryOfInts<T>(
            T entity,
            CachedFucineProperty<T> cachedFucinePropertyToPopulate,
            EntityData subEntityData,
            IDictionary dictionary,
            ContentImportLog log)
            where T : AbstractEntity<T>
        {
            foreach (DictionaryEntry dictionaryEntry in subEntityData.ValuesTable)
            {
                var num = int.Parse(dictionaryEntry.Value.ToString());
                dictionary.Add(dictionaryEntry.Key, num);
            }

            cachedFucinePropertyToPopulate.SetViaFastInvoke(entity, dictionary);
        }

        private void PopulateAsDictionaryOfLists<T>(
            T entity,
            CachedFucineProperty<T> cachedFucinePropertyToPopulate,
            Type wrapperListType,
            EntityData subEntityData,
            IDictionary dict,
            ContentImportLog log)
            where T : AbstractEntity<T>
        {
            var genericArgument = wrapperListType.GetGenericArguments()[0];
            foreach (string key in subEntityData.ValuesTable.Keys)
            {
                var defaultConstructor =
                    FactoryInstantiator.CreateObjectWithDefaultConstructor(wrapperListType) as IList;
                if (genericArgument.GetInterfaces().Contains(typeof(IQuickSpecEntity)) &&
                    subEntityData.ValuesTable[key] is string quickSpecEntityValue)
                    AddQuickSpecEntityToWrapperList(genericArgument, quickSpecEntityValue,
                        defaultConstructor, log);
                else if (subEntityData.ValuesTable[key] is ArrayList list)
                    AddFullSpecEntitiesToWrapperList(list, genericArgument, defaultConstructor, log);
                else
                    throw new ApplicationException("FucineDictionary " + cachedFucinePropertyToPopulate.LowerCaseName +
                                                   " on " + entity.GetType().Name +
                                                   " is a List<T>, but the <T> isn't drawing from strings or hashtables, but rather a " +
                                                   subEntityData.ValuesTable[key].GetType().Name);
                dict.Add(key, defaultConstructor);
            }

            cachedFucinePropertyToPopulate.SetViaFastInvoke(entity, dict);
        }

        private static void AddQuickSpecEntityToWrapperList(
            Type listMemberType,
            string quickSpecEntityValue,
            IList wrapperList,
            ContentImportLog log)
        {
            var defaultConstructor =
                FactoryInstantiator.CreateObjectWithDefaultConstructor(listMemberType) as IQuickSpecEntity;
            defaultConstructor.QuickSpec(quickSpecEntityValue);
            wrapperList.Add(defaultConstructor);
        }

        private void AddFullSpecEntitiesToWrapperList(
            ArrayList list,
            Type listMemberType,
            IList wrapperList,
            ContentImportLog log)
        {
            foreach (EntityData importDataForEntity in list)
            {
                var entity = FactoryInstantiator.CreateEntity(listMemberType, importDataForEntity, log);
                wrapperList.Add(entity);
            }
        }

        private void PopulateAsDictionaryOfEntities<T>(
            T entity,
            CachedFucineProperty<T> cachedFucinePropertyToPopulate,
            EntityData subEntityData,
            Type dictMemberType,
            IDictionary dict,
            ContentImportLog log)
            where T : AbstractEntity<T>
        {
            var vt = subEntityData.ValuesTable;
            foreach (var name in vt.Keys)
            {
                var resEntity = EntityUtils.CreateEntity(dictMemberType, vt[name], log);
                dict.Add(name, resEntity);
                cachedFucinePropertyToPopulate.SetViaFastInvoke(entity, dict);
            }
        }
    }
}