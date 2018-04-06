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

            var nullType = groups.ResolveNullHandlingType();

            Expression currentExpression = null;
            groups.ReversedForEach(group =>
            {
                if (currentExpression == null)
                {
                    var groupExpression = group.CompileGroup(NullHandling);
                    var groupExpressionLambda = Expression.Lambda(groupExpression, group.Parameter);

                    if (group.Parent == null)
                    {
                        currentExpression = groupExpressionLambda;
                        return;
                    }

                    var parent = group.Parent;
                    var parentExpression = parent.CompileGroup(NullHandling);
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
                        currentExpression = Expression.Lambda(currentExpression, group.Parameter);
                        return;
                    }

                    var parent = group.Parent;
                    var parentExpression = parent.CompileGroup(NullHandling);
                    var selectType = parent.GroupEnumerableType();
                    var currentExpressionLambda = Expression.Lambda(currentExpression, group.Parameter);
                    currentExpression = Expression.Call(typeof(Enumerable), "Select",
                        new Type[] { selectType, currentExpression.Type },
                        parentExpression, currentExpressionLambda);
                }
            });
        }
    }
}
