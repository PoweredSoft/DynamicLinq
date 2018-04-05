using System;
using System.Collections.Generic;
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

        public static LambdaExpression CompileGroup(this ExpressionParserPieceGroup group, SelectNullHandling NullHandling)
        {
            var expr = group.ParameterExpression as Expression;
            group.Pieces.ForEach(piece =>
            {
                expr = Expression.PropertyOrField(expr, piece.Name);
            });

            var ret = Expression.Lambda(expr, group.ParameterExpression);
            return ret;
        }
    }
}
