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
        private static Parser<EntityExpr> parser;

        static ExprParser()
        {
            Parser<EntityExpr> expr = null;
            var lexer = makeTokenParser(Haskell98Def);

            var literal = from mark in ch('$') from word in asString(many1(noneOf(" ,[]"))) select mark + word;
            var lident = choice(literal, lexer.Identifier, lexer.Operator);
            var operands = from o in ch('[') from w in sepBy(lazyp(() => expr), lexer.Comma) from c in ch(']') select w;

            expr = from op in lident from oper in optionOrElse(new Seq<EntityExpr>(), operands) select new EntityExpr(op, oper.ToList());
            parser = from e in expr from eof in eof select e;
        }

        public static Option<EntityExpr> Parse(string src)
        {
            return parser.Parse(src).ToOption();
        }
    }
}