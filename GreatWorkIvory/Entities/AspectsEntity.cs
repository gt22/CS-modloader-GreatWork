using System;
using SecretHistories.Core;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Entities
{
    public class AspectsEntity : AbstractEntity<AspectsEntity>, IQuickSpecEntity, IBeachcomberEntity<AspectsDictionary>
    {
        [FucineAspects]
        public AspectsDictionary Value { get; set; }
        
        public AspectsEntity(EntityData importDataForEntity, ContentImportLog log)
            : base(importDataForEntity, log)
        {
        }

        public AspectsEntity()
        {
        }

        protected override void OnPostImportForSpecificEntity(ContentImportLog log, Compendium populatedCompendium)
        {
            
        }

        public void QuickSpec(string value)
        {
            Value = new AspectsDictionary { [value] = 1 };
        }
    }
}