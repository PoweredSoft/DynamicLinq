using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Helpers;
using PoweredSoft.DynamicLinq.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
            //Case1();
            Case2();
        }

        public static void Case1()
        {
            // the expression parser.
            var ep = new ExpressionParser(typeof(Author), "Posts.Comments.Id");

            // the builder.
            var per = new PathExpressionResolver(ep);
            per.Resolve();

            // the result expression.
            var result = per.Result;
        }

        public static void Case2()
        {
            // the expression parser.
            var ep = new ExpressionParser(typeof(Author), "Posts.Author.Posts.Author.Website.Url");

            new List<Author>().AsQueryable().Select(t => new
            {
                A = t.Posts.Select(t2 => t2.Author.Posts.Select(t3 => t3.Author.Website.Url)) 
            });

            new List<Post>().AsQueryable().Select(t => new
            {
                FirstNames = t.Author == null ? new List<string>() : (t.Author.Posts == null ? new List<string>() : t.Author.Posts.Where(t2 => t2.Author != null).Select(t2 => t2.Author.FirstName)),
                PostsAuthors = t.Author == null ? new List<Author>() : (t.Author.Posts == null ? new List<Author>() : t.Author.Posts.Where(t2 => t2.Author != null).Select(t2 => t2.Author)),
                Comments = t.Comments == null ? new List<Comment>() : t.Comments,
                CommentLikes = (t.Comments == null ? new List<CommentLike>() : t.Comments.Where(t2 => t2.CommentLikes != null).SelectMany(t2 => t2.CommentLikes)),
                CommentLikeIds = (t.Comments == null ? new List<long>() : t.Comments.Where(t2 => t2.CommentLikes != null).SelectMany(t2 => t2.CommentLikes.Select(t3 => t3.Id))),
                CommentsLikes = (t.Comments == null ? new List<List<CommentLike>>() : t.Comments.Where(t2 => t2.CommentLikes != null).Select(t2 => t2.CommentLikes))
            });

            // the builder.
            var per = new PathExpressionResolver(ep);
            per.NullChecking = SelectNullHandling.Handle;
            per.Resolve();

            // the result expression.
            var result = per.Result;
        }
    }



    public class PathExpressionResolver
    {
        public SelectNullHandling NullChecking { get; set; } = SelectNullHandling.LeaveAsIs;
        public SelectCollectionHandling CollectionHandling { get; set; } = SelectCollectionHandling.LeaveAsIs;
        public ExpressionParser Parser { get; protected set; }

        public Expression Result { get; protected set; }

        public PathExpressionResolver(ExpressionParser parser)
        {
            Parser = parser;
        }

        public void Resolve()
        {
            Result = null;

            // parse the expression.
            Parser.Parse();

            // group the piece by common parameters
            var groups = Parser.GroupBySharedParameters();

            // compiled lambdas.
            var groupLambdas = groups.Select(group => group.CompileGroup(NullChecking)).ToList();
        }

        protected Expression RecursiveSelect(List<ExpressionParserPiece> pieces)
        {
            // get the last.
            var piece = pieces.Last();

            var firstEnumerableParent = ExpressionParser.GetFirstEnumerableParent(piece);
            var chainTillEnumerable = pieces.SkipWhile(t => t != firstEnumerableParent).Skip(1).ToList();
            if (chainTillEnumerable.Count == 0)
            {
                if (piece.Parent == null)
                {
                    // no parent.
                    var mostLeftExpression = Expression.PropertyOrField(Parser.Parameter, piece.Name);
                    return mostLeftExpression;
                }
                else
                {
                    // we have a parent its probably a enumerable.
                    var collectionParentParameter = Expression.Parameter(piece.Parent.EnumerableType);
                    var memberOf = Expression.PropertyOrField(collectionParentParameter, piece.Name);
                }
            }
            else
            {
                // we have a simple chain to resolve.
                var chainTillEnumerableParameter = ResolveParameter(chainTillEnumerable);
                var chainTillEnumerableExpression = BuildSimpleChainExpression(chainTillEnumerable, chainTillEnumerableParameter);
                var chainTillEnumerableLambda = Expression.Lambda(chainTillEnumerableExpression, chainTillEnumerableParameter);

                // get parent to glue with.
                var nextList = pieces.Take(pieces.IndexOf(firstEnumerableParent)+1).ToList();
                var parent = RecursiveSelect(nextList);

                // glue.
                var gluedResult = Expression.Call(typeof(Enumerable),
                    "Select",
                    new Type[] { firstEnumerableParent.EnumerableType, chainTillEnumerableExpression.Type },
                    parent, chainTillEnumerableLambda);

                return gluedResult;
            }
            
            throw null;
            
        }

        private ExpressionParserPiece GetFirstEnumerableParent(ExpressionParserPiece piece)
        {
            if (piece.Parent == null)
                return null;

            if (piece.Parent.IsGenericEnumerable)
                return piece.Parent;

            return GetFirstEnumerableParent(piece.Parent);
        }

        private Expression NullCheckTernary(Expression left, Expression right, ParameterExpression parameter)
        {
            var lastPiece = Parser.Pieces.Last();
            var lastPieceType = !lastPiece.IsGenericEnumerable ? lastPiece.Type : lastPiece.EnumerableType;

            var escapeType = typeof(List<>).MakeGenericType(lastPieceType);
            var typeMatch = typeof(IEnumerable<>).MakeGenericType(lastPieceType);
            var ifTrueExpression = Expression.New(escapeType);
            var testExpression = Expression.Equal(left, Expression.Constant(null));

            var condition = Expression.Condition(testExpression, ifTrueExpression, right, typeMatch);
            var lambda = Expression.Lambda(condition, parameter);
            return lambda;
        }

        private Expression BuildNullCheckConditionExpressionForWhere(List<ExpressionParserPiece> currentSimpleChain, ParameterExpression parameter)
        {
            var path = string.Join(".", currentSimpleChain.Select(t => t.Name).Take(currentSimpleChain.Count - 1));
            var where = QueryableHelpers.CreateConditionExpression(parameter.Type, path, ConditionOperators.NotEqual, null, QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, 
                nullChecking: true, parameter: parameter);
            return where;
        }

        private ParameterExpression ResolveParameter(List<ExpressionParserPiece> currentSimpleChain)
        {
            var first = currentSimpleChain.First();
            if (first.Parent == null)
                return Parser.Parameter;

            if (first.Parent.IsGenericEnumerable)
                return Expression.Parameter(first.Parent.EnumerableType);

            throw new NotSupportedException();
        }

        private Expression BuildSimpleChainExpression(List<ExpressionParserPiece> chain, ParameterExpression parameter)
        {
            var ret = parameter as Expression;
            chain.ForEach(p =>
            {
                var me = Expression.PropertyOrField(ret, p.Name);
                ret = me;
            });

            return ret;
        }
    }
}
