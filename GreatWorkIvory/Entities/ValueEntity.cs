using System;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Entities
{
    public class ValueEntity<T> : AbstractEntity<ValueEntity<T>>, IQuickSpecEntity, IBeachcomberEntity<T>
    {
        
        [FucineValue]
        public T Value { get; set; }

        public ValueEntity(EntityData importDataForEntity, ContentImportLog log) : base(importDataForEntity, log)
        {
        }

        public ValueEntity()
        {
        }

        protected override void OnPostImportForSpecificEntity(ContentImportLog log, Compendium populatedCompendium)
        {
            
        }

        public void QuickSpec(string value)
        {
            Value = (T) Convert.ChangeType(value, typeof(T));
        }
    }
}