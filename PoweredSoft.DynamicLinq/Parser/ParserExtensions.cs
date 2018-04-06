using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PoweredSoft.DynamicLinq.Parser
{
    public static class ParserExtensions
    {
        public static ExpressionParserPiece FirstEnumerableParent(this ExpressionParserPiece piece)
        {
            var result = ExpressionParser.GetFirstEnumerableParent(piece);
            return result;
        }

        public static Expression CompileGroup(this ExpressionParserPieceGroup group, SelectNullHandling NullHandling)
        {
            var expr = group.Parameter as Expression;
            group.Pieces.ForEach(piece =>
            {
                expr = Expression.PropertyOrField(expr, piece.Name);
            });
            return expr;
        }

        public static Type GroupEnumerableType(this ExpressionParserPieceGroup group)
        {
            return group.Pieces.Last().EnumerableType;
        }
    }
}
