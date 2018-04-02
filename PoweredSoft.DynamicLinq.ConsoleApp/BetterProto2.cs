using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.ConsoleApp
{
    public class BetterProto2
    {
        /*
         *  (parent, innerExpression, innerExpressionLambda) =>
                {
                    var listGenericArgumentType = parent.Type.GetGenericArguments().First();
                    Expression ret = null;
                    if (selectCollectionHandling == SelectCollectionHandling.LeaveAsIs || !QueryableHelpers.IsGenericEnumerable(innerExpression))
                        ret = Expression.Call(typeof(Enumerable), "Select", new Type[] { listGenericArgumentType, innerExpression.Type }, parent, innerExpressionLambda);
                    else
                        ret = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { listGenericArgumentType, innerExpression.Type.GenericTypeArguments.First() }, parent, innerExpressionLambda);

                    return ret;
                }
         */
        public static void Run()
        {
            var ep = new ExpressionParser(typeof(Author), "Posts.Comments.Id");
            ep.Parse();
        }
    }

    public class ExpressionParserPiece
    {
        public ParameterExpression Parameter { get; set; }
        public MemberExpression MemberExpression { get; set; }
        public ExpressionParserPiece Parent { get; set; }

        public bool IsGenericEnumerable => QueryableHelpers.IsGenericEnumerable(MemberExpression);
        public Type EnumerableType => MemberExpression.Type.GenericTypeArguments.FirstOrDefault();
    }

    public class ExpressionParser
    {
        public ParameterExpression Parameter { get; protected set; }
        public string Path { get; set; }
        public List<ExpressionParserPiece> Pieces { get; set; } = new List<ExpressionParserPiece>();

        public ExpressionParser(Type type, string path) : this(Expression.Parameter(type), path)
        {
            
        }

        public ExpressionParser(ParameterExpression parameter, string path)
        {
            Parameter = parameter;
            Path = path;
        }

        public void Parse()
        {
            Pieces.Clear();

            var param = Parameter;
            var pathPieces = Path.Split('.').ToList();
            ExpressionParserPiece parent = null;

            pathPieces.ForEach(pp =>
            {
                var memberExpression = Expression.PropertyOrField(param, pp);
                var current = new ExpressionParserPiece
                {
                    Parameter = param,
                    MemberExpression = memberExpression,
                    Parent = parent
                };

                Pieces.Add(current);
                param = ResolveNextParam(current);
                parent = current;
            });
        }

        private ParameterExpression ResolveNextParam(ExpressionParserPiece current)
        {
            var type = current.IsGenericEnumerable ? current.EnumerableType : current.MemberExpression.Type;
            var result = Expression.Parameter(type);
            return result;
        }
    }
}
