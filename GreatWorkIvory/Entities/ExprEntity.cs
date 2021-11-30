using System;
using System.Collections.Generic;
using GreatWorkIvory.Expressions;
using LanguageExt;
using SecretHistories.Fucine;
using SecretHistories.Fucine.DataImport;

namespace GreatWorkIvory.Entities
{
    public class ExprEntity : AbstractEntity<ExprEntity>, IQuickSpecEntity
    {

        [FucineValue]
        public string Op { get; set; }
        
        [FucineList]
        public List<ExprEntity> Operands { get; set; }

        public ExprEntity(EntityData importDataForEntity, ContentImportLog log) : base(importDataForEntity, log)
        {
        }

        public ExprEntity(string op, List<ExprEntity> operands)
        {
            Op = op;
            Operands = operands;
        }

        public ExprEntity(string op)
        {
            Op = op;
            Operands = new List<ExprEntity>();
        }

        public ExprEntity()
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
            var res = ExprParser.Parse(value);
            if (res.Case is SomeCase<ExprEntity> e)
            {
                Op = e.Value.Op;
                Operands = e.Value.Operands;
            }
            else
            {
                throw new ArgumentException(value);
            }
        }
    }
}