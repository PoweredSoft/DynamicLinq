﻿using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Helpers;
using PoweredSoft.DynamicLinq.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.ConsoleApp
{
    public class SelectExpression
    {
        static void Main(string[] args)
        {
            var q = new List<Author>().AsQueryable();
            q.Select(t => new
            {
                Ids = t.Posts.SelectMany(t2 => t2.Comments.Select(t3 => t3.CommentLikes))
            });

            var expressionParser = new ExpressionParser(typeof(Author), "Posts.Comments.CommentLikes");
            expressionParser.Parse();

            var finalExpression = CreateSelectExpressionFromParsed(expressionParser, SelectCollectionHandling.Flatten, SelectNullHandling.New);
        }

        public static Expression CreateSelectExpressionFromParsed(ExpressionParser expressionParser, 
            SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs, 
            SelectNullHandling nullChecking = SelectNullHandling.LeaveAsIs)
        {
            Type nullHandlingRightType = null;
            Expression nullHandlingNullValue = null;
            
            if (nullChecking != SelectNullHandling.LeaveAsIs)
            {
                var withoutNullCheckResult = CreateSelectExpressionFromParsed(expressionParser, SelectCollectionHandling.Flatten, SelectNullHandling.LeaveAsIs);
                if (QueryableHelpers.IsGenericEnumerable(withoutNullCheckResult.Type))
                    nullHandlingRightType = typeof(List<>).MakeGenericType(withoutNullCheckResult.Type.GenericTypeArguments.First());
                else
                    nullHandlingRightType = withoutNullCheckResult.Type;

                nullHandlingNullValue = nullChecking == SelectNullHandling.Default
                    ? Expression.Default(nullHandlingRightType) as Expression
                    : Expression.New(nullHandlingRightType) as Expression;
            }

            // reversed :)
            var reversedCopy = expressionParser.Groups.Select(t => t).ToList();
            reversedCopy.Reverse();

            MethodCallExpression lastSelectExpression = null;
            Expression ret = null;

            foreach(var t in reversedCopy)
            {
                if (true == t.Parent?.LastPiece().IsGenericEnumerable())
                {
                    if (lastSelectExpression == null)
                        lastSelectExpression = RegroupSelectExpressions(t, selectCollectionHandling);
                    else
                        lastSelectExpression = RegroupSelectExpressions(t, lastSelectExpression, selectCollectionHandling);
                }
            }

            ret = lastSelectExpression;
            return ret;
        }

        public static MethodCallExpression RegroupSelectExpressions(ExpressionParameterGroup group, SelectCollectionHandling selectCollectionHandling)
        {
            MethodCallExpression ret = null;
            var lambda = group.CreateLambda();
            var parentExpression = group.Parent.LastExpression();
            var isParentExpressionGenericEnumerable = QueryableHelpers.IsGenericEnumerable(parentExpression);
            var parentExpressionGenericEnumerableType = isParentExpressionGenericEnumerable ? parentExpression.Type.GenericTypeArguments.First() : null;
            var lastPiece = group.LastPiece();

            if (selectCollectionHandling == SelectCollectionHandling.LeaveAsIs || !lastPiece.IsGenericEnumerable())
                ret = Expression.Call(typeof(Enumerable), "Select", new Type[] { parentExpressionGenericEnumerableType, lastPiece.Expression.Type }, parentExpression, lambda);
            else
                ret = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { parentExpressionGenericEnumerableType, lastPiece.GetGenericEnumerableType() }, parentExpression, lambda);

            return ret;
        }

        public static MethodCallExpression RegroupSelectExpressions(ExpressionParameterGroup group, MethodCallExpression innerSelect, SelectCollectionHandling selectCollectionHandling)
        {
            var parent = group.Parent;
            var parentLastPiece = parent.LastPiece();

            MethodCallExpression ret = null;
            var lambda = Expression.Lambda(innerSelect, group.Parameter);
            if (selectCollectionHandling == SelectCollectionHandling.LeaveAsIs)
                ret = Expression.Call(typeof(Enumerable), "Select", new Type[] { parentLastPiece.GetGenericEnumerableType(), innerSelect.Type }, parentLastPiece.Expression, lambda);
            else
                ret = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { parentLastPiece.GetGenericEnumerableType(), innerSelect.Type.GenericTypeArguments.First() }, parentLastPiece.Expression, lambda);

            return ret;
        }



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
    }
}