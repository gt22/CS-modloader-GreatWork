using GreatWorkIvory.Entities;
using LanguageExt.Parsec;
using LanguageExt;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Token;
using static LanguageExt.Parsec.Language;

namespace GreatWorkIvory.Expressions
{
    public class ExprParser
    {
        private static Parser<ExprEntity> parser;

        static ExprParser()
        {
            Parser<ExprEntity> expr = null;
            var lexer = makeTokenParser(Haskell98Def);

            var literal = 
                from mark in ch('$') 
                from word in asString(many1(noneOf(" ,[]"))) 
                select mark + word;
            
            var operation = choice(literal, lexer.Identifier, lexer.Operator);
            
            var operands = 
                from o in ch('[')
                // ReSharper disable once AccessToModifiedClosure
                from w in sepBy(lazyp(() => expr), lexer.Comma)
                from c in ch(']')
                select w;

            expr = 
                from op in operation
                from oper in optionOrElse(Seq.empty<ExprEntity>(), operands)
                select new ExprEntity(op, oper.ToList());
            
            parser = 
                from e in expr 
                from eof in eof 
                select e;
        }

        public static Option<ExprEntity> Parse(string src)
        {
            return parser.Parse(src).ToOption();
        }
    }
}