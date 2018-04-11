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

      

        public static Type GroupEnumerableType(this ExpressionParserPieceGroup group)
        {
            return group.Pieces.Last().EnumerableType;
        }

        public static Type ResolveNullHandlingType(this List<ExpressionParserPieceGroup> groups)
        {
            if (groups.Count() == 1)
            {
                throw new NotImplementedException();
            }

            var type = groups.Last().Pieces.Last().Type;
            return typeof(IEnumerable<>).MakeGenericType(type);
        }
    }
}
