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
            var ep = new ExpressionParser(typeof(Author), "Posts.Author.Website.Url");

            new List<Author>().AsQueryable().Select(t => new
            {
                A = t.Posts.Select(t2 => t2.Author.Posts.Select(t3 => t3.Author.Website.Url)) ,
                B = t.Posts.Where(t2 => t2.Author != null).Select(t2 => t2.Author.Posts.Where(t3 => t3.Author != null && t3.Author.Website != null).Select(t3 => t3.Author.Website.Url))
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
            per.NullHandling = SelectNullHandling.Handle;
            per.Resolve();

            // the result expression.
            var result = per.Result;
        }
    }



    public class PathExpressionResolver
    {
        public SelectNullHandling NullHandling { get; set; } = SelectNullHandling.LeaveAsIs;
        public SelectCollectionHandling CollectionHandling { get; set; } = SelectCollectionHandling.LeaveAsIs;
        public ExpressionParser Parser { get; protected set; }

        public Expression Result { get; protected set; }
        public Type NullType { get; set; }

        public PathExpressionResolver(ExpressionParser parser)
        {
            Parser = parser;
        }

        protected Expression CompileGroup(ExpressionParserPieceGroup group, SelectNullHandling NullHandling)
        {
            var expr = group.Parameter as Expression;
            group.Pieces.ForEach(piece =>
            {
                expr = Expression.PropertyOrField(expr, piece.Name);
            });
            return expr;
        }

        public void Resolve()
        {
            Result = null;

            // parse the expression.
            Parser.Parse();

            // group the piece by common parameters
            var groups = Parser.GroupBySharedParameters();

            NullType = groups.ResolveNullHandlingType();

            Expression currentExpression = null;
            foreach(var group in groups.Reversed())
            { 
                if (currentExpression == null)
                {
                    var groupExpression = CompileGroup(group, NullHandling);
                    var groupExpressionLambda = Expression.Lambda(groupExpression, group.Parameter);

                    if (group.Parent == null)
                    {
                        currentExpression = groupExpressionLambda;
                        continue;
                    }

                    var parent = group.Parent;
                    var parentExpression = CompileGroup(parent, NullHandling);

                    // check null with where.
                    if (NullHandling != SelectNullHandling.LeaveAsIs)
                        parentExpression = CheckNullOnEnumerableParent(group, parent, parentExpression);

                    // the select expression.
                    var selectType = parent.GroupEnumerableType();
                    var selectExpression = Expression.Call(typeof(Enumerable), "Select", 
                        new Type[] { selectType, groupExpression.Type }, 
                        parentExpression, groupExpressionLambda);
                    currentExpression = selectExpression;
                }
                else
                {
                    if (group.Parent == null)
                    {
                        if (NullHandling != SelectNullHandling.LeaveAsIs)
                            currentExpression = CheckNullOnFirstGroup(group, currentExpression);

                        currentExpression = Expression.Lambda(currentExpression, group.Parameter);
                        continue;
                    }

                    var parent = group.Parent;
                    var parentExpression = CompileGroup(parent, NullHandling);
                    var selectType = parent.GroupEnumerableType();


                    if (NullHandling != SelectNullHandling.LeaveAsIs)
                        parentExpression = CheckNullOnEnumerableParent(group, parent, parentExpression);

                    var currentExpressionLambda = Expression.Lambda(currentExpression, group.Parameter);
                    currentExpression = Expression.Call(typeof(Enumerable), "Select",
                        new Type[] { selectType, currentExpression.Type },
                        parentExpression, currentExpressionLambda);
                }
            }

            Result = currentExpression;
        }

        private Expression CheckNullOnFirstGroup(ExpressionParserPieceGroup group, Expression currentExpression)
        {
            var path = string.Join(".", group.Pieces.Select(t => t.Name));
            var whereExpression = QueryableHelpers.CreateConditionExpression(group.Parameter.Type, path,
                    ConditionOperators.NotEqual, null, QueryConvertStrategy.ConvertConstantToComparedPropertyOrField,
                    parameter: group.Parameter, nullChecking: true);

            var whereBodyExpression = (whereExpression as LambdaExpression).Body;

            Expression ifTrueExpression = null;
            if (QueryableHelpers.IsGenericEnumerable(NullType))
            {
                var listType = typeof(List<>).MakeGenericType(NullType.GenericTypeArguments.First());
                ifTrueExpression = Expression.New(listType);

            }
            else
            {
                ifTrueExpression = Expression.Default(NullType);
            }

            return Expression.Condition(whereBodyExpression, ifTrueExpression, currentExpression, currentExpression.Type);
        }

        private static Expression CheckNullOnEnumerableParent(ExpressionParserPieceGroup group, ExpressionParserPieceGroup parent, Expression parentExpression)
        {
            if (group.Pieces.Count > 1)
            {
                var path = string.Join(".", group.Pieces.Take(group.Pieces.Count - 1).Select(T => T.Name));
                var whereExpression = QueryableHelpers.CreateConditionExpression(group.Parameter.Type, path,
                    ConditionOperators.NotEqual, null, QueryConvertStrategy.ConvertConstantToComparedPropertyOrField,
                    parameter: group.Parameter, nullChecking: true);

                //public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate);
                parentExpression = Expression.Call(typeof(Enumerable), "Where",
                    new Type[] { parent.GroupEnumerableType() },
                    parentExpression, whereExpression);
            }
            else
            {

            }

            return parentExpression;
        }
    }
}
