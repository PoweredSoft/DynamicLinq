using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Helpers;
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
            var ep = new ExpressionParser(typeof(Post), "Author.Posts.Author.Posts.Author.FirstName");

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
            per.NullChecking = SelectNullHandling.New;
            per.Resolve();

            // the result expression.
            var result = per.Result;
        }
    }

    public class ExpressionParserPiece
    {
        public Type Type { get; set; }
        public bool IsGenericEnumerable { get; set; }
        public Type EnumerableType { get; set; }
        public ExpressionParserPiece Parent { get; set; }
        public string Name { get; internal set; }
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

            // reverse foreach
            var last = Parser.Pieces.Last();

            // reverse recursive.
            Result = RecursiveSelect(last);
        }

        protected Expression RecursiveSelect(ExpressionParserPiece piece)
        {
            if (piece.Parent == null)
            {
                var parameter = Parser.Parameter;
                var me = Expression.PropertyOrField(Parser.Parameter, piece.Name);
                return me;
            }
            else if (piece.Parent.IsGenericEnumerable)
            {
                var indexOfParentPiece = Parser.Pieces.IndexOf(piece);
                var currentSimpleChain = Parser.Pieces.Skip(indexOfParentPiece).TakeWhile(t => !t.IsGenericEnumerable).ToList();     
                var parameter = ResolveParameter(currentSimpleChain);
                var currentSimpleChainExpression = BuildSimpleChainExpression(currentSimpleChain, parameter);
                var currentSimpleChainExpressionLambda = Expression.Lambda(currentSimpleChainExpression, parameter);

                // the left before the parent.
                var left = RecursiveSelect(piece.Parent);

                // the parent.
                var parentExpression = Expression.PropertyOrField(left, piece.Parent.Name) as Expression;

                if (NullChecking != SelectNullHandling.LeaveAsIs)
                {
                    var nullCheckParameter = ResolveParameter(currentSimpleChain);
                    var nullCheckConditionExpression = BuildNullCheckConditionExpression(currentSimpleChain, nullCheckParameter);
                    var whereExpression = Expression.Call(typeof(Enumerable), "Where", new[] { piece.Parent.EnumerableType }, parentExpression, nullCheckConditionExpression);
                    parentExpression = whereExpression as Expression;
                }

                // select.
                var gluedTogetherExpression = Expression.Call(typeof(Enumerable),
                    "Select",
                    new Type[] { piece.Parent.EnumerableType, currentSimpleChainExpression.Type },
                    parentExpression, currentSimpleChainExpressionLambda);

                // add current to parent?
                return gluedTogetherExpression;
            }

            // skip.
            return RecursiveSelect(piece.Parent);
        }

        /*
        private void ResolveNullCheckingRightType()
        {
            if (NullChecking == SelectNullHandling.LeaveAsIs)
                return;
            
            // last piece.
            var lastPiece = Parser.Pieces.Last();
            var anyCollections = Parser.Pieces.Any(t => t.IsGenericEnumerable);

            Type subjectType = null;
            if (anyCollections)
                subjectType = typeof(List<>).MakeGenericType(lastPiece.EnumerableType);
            else
                subjectType = lastPiece.Type;

            if (NullChecking == SelectNullHandling.Default)
                NullCheckValueExpression = Expression.Default(subjectType);
            else
                NullCheckValueExpression = Expression.New(subjectType);
        }*/

        private void HandleCollection(ExpressionParserPiece piece, int index)
        {
            // simple parent.

            // the current simple chain.
            var currentSimpleChain = Parser.Pieces.Skip(index - 1).Where(t => !t.IsGenericEnumerable).ToList();
            
            // the parameter being used to select.
            var parameter = ResolveParameter(currentSimpleChain);

            // this is the simple chain to select.
            var currentSimpleChainExpression = BuildSimpleChainExpression(currentSimpleChain, parameter);

            // add where :)
            if (NullChecking != SelectNullHandling.LeaveAsIs)
            {
                var nullCheckParameter = ResolveParameter(currentSimpleChain);
                var nullCheckWhereForChain = BuildNullCheckConditionExpression(currentSimpleChain, nullCheckParameter);
            }
            else
            {
                Result = currentSimpleChainExpression;
            }
        }

        private Expression BuildNullCheckConditionExpression(List<ExpressionParserPiece> currentSimpleChain, ParameterExpression parameter)
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

        private Expression BuildSimpleChainExpression(List<ExpressionParserPiece> currentSimpleChain, ParameterExpression parameter)
        {
            var ret = parameter as Expression;
            currentSimpleChain.ForEach(p =>
            {
                var me = Expression.PropertyOrField(ret, p.Name);
                ret = me;
            });

            return ret;
        }

        /*
        public void HandleNoPreviousResultCollection(ExpressionParserPiece piece, int index)
        {
            MethodCallExpression result = null;

            // create the lambda.
            var lambda = ResolveLambda(piece);

            if (CollectionHandling == SelectCollectionHandling.LeaveAsIs || !piece.IsGenericEnumerable)
                result = Expression.Call(typeof(Enumerable),
                    "Select",
                    new Type[] { piece.Parent.EnumerableType, piece.MemberExpression.Type },
                    piece.Parent.MemberExpression, lambda);
            else
                result = Expression.Call(typeof(Enumerable),
                    "SelectMany",
                    new Type[] { piece.Parent.EnumerableType, piece.EnumerableType },
                    piece.Parent.MemberExpression, lambda);

            Result = result;
        }

        public void HandlePreviousResultCollection(ExpressionParserPiece piece, int index)
        {
            var pieceParameter = ResolvePieceParameter(piece);

            MethodCallExpression result = null;
            var lambda = Expression.Lambda(Result, ResolvePieceParameter(piece));
            if (CollectionHandling == SelectCollectionHandling.LeaveAsIs)
                result = Expression.Call(typeof(Enumerable), "Select", 
                    new Type[] { piece.Parent.EnumerableType, Result.Type }, 
                    piece.Parent.MemberExpression, lambda);
            else
                result = Expression.Call(typeof(Enumerable),
                    "SelectMany",
                    new Type[] { piece.Parent.EnumerableType, Result.Type.GenericTypeArguments.First() },
                    piece.Parent.MemberExpression, lambda);

            Result = result;
        }*/
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
                    EnumerableType = memberExpression.Type.GenericTypeArguments.FirstOrDefault(),
                    Parent = parent,
                    Name = pp
                };

                Pieces.Add(current);

                // for next iteration.
                param = Expression.Parameter(current.IsGenericEnumerable ? current.EnumerableType : current.Type);
                parent = current;
            });
        }
    }
}
