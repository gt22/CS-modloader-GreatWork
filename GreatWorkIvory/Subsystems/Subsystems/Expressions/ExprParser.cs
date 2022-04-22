using System;
using System.Collections.Generic;
using System.Linq;
using GreatWorkIvory.Entities;
using LanguageExt.Parsec;
using LanguageExt;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;
using static LanguageExt.Parsec.Token;
using static LanguageExt.Parsec.Language;

namespace GreatWorkIvory.Expressions
{
    public static class ExprParser
    {
        private static readonly Parser<ExprEntity> Parser;

        private static Func<ExprEntity, ExprEntity, ExprEntity> Join(string op)
        {
            return (a, b) => new ExprEntity(op, new List<ExprEntity> { a, b });
        }

        static ExprParser()
        {
            var lexer = makeTokenParser(Haskell98Def);

            var literal =
                from mark in ch('$')
                from word in asString(many1(satisfy(x => !char.IsWhiteSpace(x) && !",[]".Contains(x))))
                select mark + word;

            var operation = choice(literal, lexer.Identifier, lexer.Operator);

            Parser<ExprEntity> exprPlain = null;
            Parser<ExprEntity> exprSet = null;
            Parser<ExprEntity> parenthesisedExpr = null;

            // ReSharper disable once AccessToModifiedClosure
            var exprSingle = between(spaces, spaces, lazyp(() => either(parenthesisedExpr, exprPlain)));
            // ReSharper disable once AccessToModifiedClosure
            var expr = between(spaces, spaces, lazyp(() => either(attempt(exprSet), exprSingle)));

            var operands = between(ch('['), ch(']'), sepBy(expr, lexer.Comma));

            exprPlain =
                from op in operation
                from operand in optionOrElse(Seq.empty<ExprEntity>(), operands)
                select new ExprEntity(op, operand.ToList());

            exprSet = chainl1(exprSingle, lexer.Operator.Select(Join));

            parenthesisedExpr = between(ch('('), ch(')'), expr);

            Parser =
                from e in expr
                from eof in eof
                select e;
        }

        public static Option<ExprEntity> Parse(string src)
        {
            return Parser.Parse(src).ToOption();
        }
        
        public static void Main (string[] args) {
            Console.WriteLine (Parser.Parse("aspect[$lantern]").ToOption());
        }
        
    }
}