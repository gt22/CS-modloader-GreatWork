using System;
using System.Collections.Generic;
using System.Linq;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Expressions
{
    [FucineImportable("expressions_test")]
    public class EntityExpr : AbstractEntity<EntityExpr>, IQuickSpecEntity
    {

        [FucineValue]
        public string Op { get; set; }
        
        [FucineList]
        public List<EntityExpr> Operands { get; set; }

        public EntityExpr(EntityData importDataForEntity, ContentImportLog log) : base(importDataForEntity, log)
        {
        }

        public EntityExpr(string op, List<EntityExpr> operands)
        {
            Op = op;
            Operands = operands;
        }

        public EntityExpr(string op)
        {
            Op = op;
            Operands = new List<EntityExpr>();
        }

        public EntityExpr()
        {
        }

        protected override void OnPostImportForSpecificEntity(ContentImportLog log, Compendium populatedCompendium)
        {
        }

        public override string ToString()
        {
            return $"{Op}[{string.Join(", ", Operands)}]";
        }

        public void QuickSpec(string value)
        {
            // var res = ExprParser.Parse(value);
            
        }
    }
}