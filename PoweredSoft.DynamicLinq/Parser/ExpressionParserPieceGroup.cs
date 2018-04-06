using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PoweredSoft.DynamicLinq.Parser
{
    public class ExpressionParserPieceGroup
    {
        public List<ExpressionParserPiece> Pieces { get; set; } = new List<ExpressionParserPiece>();
        public ParameterExpression Parameter { get; set; }
        public ExpressionParserPieceGroup Parent { get; set; }

#if DEBUG
        public override string ToString() => $"{Parameter?.ToString()} is {Parameter?.Type} | {(Pieces == null ? "" : string.Join(" -> ", Pieces.Select(t2 => t2.ToString())))}";

        public object CompileSimpleExpress(SelectNullHandling nullHandling)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
