using System.Collections.Generic;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Entities
{
    public class DictEntity<T> : AbstractEntity<DictEntity<T>>, IBeachcomberEntity<Dictionary<string, T>>
    {
        [FucineDict]
        public Dictionary<string, T> Value { get; set;  }

        public DictEntity(EntityData importDataForEntity, ContentImportLog log) : base(importDataForEntity, log)
        {
        }

        protected override void OnPostImportForSpecificEntity(ContentImportLog log, Compendium populatedCompendium)
        {
            
        }

        
    }
}