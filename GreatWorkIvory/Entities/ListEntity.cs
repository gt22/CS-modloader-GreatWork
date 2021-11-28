using System.Collections.Generic;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Entities
{
    public class ListEntity<T> : AbstractEntity<ListEntity<T>>, IBeachcomberEntity<List<T>>
    {

        [FucineList] 
        public List<T> Value { get; set; }

        public ListEntity(EntityData importDataForEntity, ContentImportLog log) : base(importDataForEntity, log)
        {
        }

        protected override void OnPostImportForSpecificEntity(ContentImportLog log, Compendium populatedCompendium)
        {
            
        }
    }
}