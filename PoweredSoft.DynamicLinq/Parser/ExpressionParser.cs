using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PoweredSoft.DynamicLinq.Parser
{
    public class ExpressionParser
    {
        public ParameterExpression Parameter { get; protected set; }
        public string Path { get; set; }
        public List<ExpressionParserPiece> Pieces { get; set; } = new List<ExpressionParserPiece>();
        public bool IsParsed => Pieces?.Count > 0;

        public ExpressionParser(Type type, string path) : this(Expression.Parameter(type), path)
        {
            
        }

        public ExpressionParser(ParameterExpression parameter, string path)
        {
            Parameter = parameter;
            Path = path;
        }

        public static ExpressionParserPiece GetFirstEnumerableParent(ExpressionParserPiece piece)
        {
            if (piece.Parent == null)
                return null;

            if (piece.Parent.IsGenericEnumerable)
                return piece.Parent;

            return GetFirstEnumerableParent(piece.Parent);
        }

        public void Parse()
        {
            Pieces.Clear();

            var pathPieces = Path.Split('.').ToList();
            var param = Parameter;
            ExpressionParserPiece parent = null;

            pathPieces.ForEach(pp =>
            {
                var memberExpression = Expression.PropertyOrField(param, pp);
                var current = new ExpressionParserPiece
                {
                    Type = memberExpression.Type,
                    IsGenericEnumerable = QueryableHelpers.IsGenericEnumerable(memberExpression),
                    EnumerableType = QueryableHelpers.GetTypeOfEnumerable(memberExpression.Type, false),
                    Parent = parent,
                    Name = pp
                };

                Pieces.Add(current);

                // for next iteration.
                param = Expression.Parameter(current.IsGenericEnumerable ? current.EnumerableType : current.Type);
                parent = current;
            });
        }

        private ExpressionParserPieceGroup CreateAndAddGroup(List<ExpressionParserPieceGroup> groups, ParameterExpression parameter, ExpressionParserPieceGroup parent)
        {
            var group = new ExpressionParserPieceGroup();
            group.Parameter = parameter;
            group.Parent = parent;
            groups.Add(group);
            return group;
        }

        public List<ExpressionParserPieceGroup> GroupBySharedParameters()
        {
            var groups = new List<ExpressionParserPieceGroup>();

            var group = CreateAndAddGroup(groups, Parameter, null);
            Pieces.ForEach(piece =>
            {
                group.Pieces.Add(piece);
                if (piece.IsGenericEnumerable)
                    group = CreateAndAddGroup(groups, Expression.Parameter(piece.EnumerableType), group);
            });

            // if the last piece is empty.
            if (group.Pieces.Count == 0)
                groups.Remove(group);

            return groups;
        }
    }
}
