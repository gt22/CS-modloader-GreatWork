using System;
using System.Collections.Generic;
using System.Reflection;
using SecretHistories.Constants.Modding;
using SecretHistories.Fucine;

namespace GreatWorkIvory.Events.EventTypes
{
    //TODO
    public class CompendiumEvent : Event
    {
        public readonly Compendium Compendium;
        public readonly string Culture;
        public readonly ContentImportLog Log;

        public CompendiumEvent(Compendium compendium, string culture, ContentImportLog log)
        {
            Compendium = compendium;
            Culture = culture;
            Log = log;
        }

        public class Begin : CompendiumEvent
        {
            public Begin(Compendium compendium, string culture, ContentImportLog log) : base(compendium, culture, log)
            {
            }
        }

        public class ModIndexing : CompendiumEvent
        {
            public readonly List<DataFileLoader> Content, Loc;

            public void AddLoaders(DataFileLoader content, DataFileLoader loc)
            {
                Content.Add(content);
                Loc.Add(loc);
            }

            public void AddLoaders(string contentDir, string locDir)
            {
                var content = new DataFileLoader(contentDir);
                content.LoadFilesFromAssignedFolder(Log);
                var loc = new DataFileLoader(locDir);
                loc.LoadFilesFromAssignedFolder(Log);
                AddLoaders(content, loc);
            }

            public void AddMod(Mod mod)
            {
                AddLoaders(mod.ContentFolder, mod.LocFolder);
            }
            
            public ModIndexing(Compendium compendium, string culture, ContentImportLog log,
                List<DataFileLoader> content, List<DataFileLoader> loc) : base(compendium, culture, log)
            {
                Content = content;
                Loc = loc;
            }

            public class Pre : ModIndexing
            {
                public Pre(Compendium compendium, string culture, ContentImportLog log, List<DataFileLoader> content,
                    List<DataFileLoader> loc) : base(compendium, culture, log, content, loc)
                {
                }
            }

            public class Post : ModIndexing
            {
                public Post(Compendium compendium, string culture, ContentImportLog log, List<DataFileLoader> content,
                    List<DataFileLoader> loc) : base(compendium, culture, log, content, loc)
                {
                }
            }
        }

        public class TypeRegistry : CompendiumEvent
        {
            public readonly Dictionary<string, EntityTypeDataLoader> Loaders;
            public readonly List<Type> Types;

            public TypeRegistry(Compendium compendium, string culture, ContentImportLog log,
                Dictionary<string, EntityTypeDataLoader> loaders, List<Type> types) : base(compendium, culture, log)
            {
                Loaders = loaders;
                Types = types;
            }

            public bool TryAddEntityType(Type type)
            {
                var customAttribute = (FucineImportable) type.GetCustomAttribute(typeof(FucineImportable), false);
                if (customAttribute == null) return false;
                var entityTypeDataLoader = new EntityTypeDataLoader(type, customAttribute.TaggedAs, Culture, Log);
                Loaders.Add(customAttribute.TaggedAs.ToLower(), entityTypeDataLoader);
                Types.Add(type);
                return true;
            }
            
            public class Pre : TypeRegistry
            {
                public Pre(Compendium compendium, string culture, ContentImportLog log,
                    Dictionary<string, EntityTypeDataLoader> loaders, List<Type> types) : base(compendium, culture, log,
                    loaders, types)
                {
                }
            }

            public class Post : TypeRegistry
            {
                public Post(Compendium compendium, string culture, ContentImportLog log,
                    Dictionary<string, EntityTypeDataLoader> loaders, List<Type> types) : base(compendium, culture, log,
                    loaders, types)
                {
                }
            }
        }

        public class End : CompendiumEvent
        {
            public End(Compendium compendium, string culture, ContentImportLog log) : base(compendium, culture, log)
            {
            }
        }
    }
}