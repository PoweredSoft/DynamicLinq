using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PoweredSoft.DynamicLinq.Helpers
{
    public static class QueryablePathHelpers
    {
        public delegate Expression SimplePathNavigated(Expression before, MemberExpression member, bool isFirst, bool isLast);
        public delegate Expression CollectionPathNavigated(Expression before, MemberExpression member, bool isFirst, bool isLast);
        public delegate Expression CollectionHandling(Expression parent, Expression innerExpression, LambdaExpression innerExpressionLambda);

        internal static Expression InternalNavigatePath(Expression before, List<string> parts, SimplePathNavigated simple, CollectionPathNavigated collection, CollectionHandling collectionHandling)
        {
            var isFirst = before is ParameterExpression;
            var isLast = parts.Count() == 1;
            var currentExpression = Expression.PropertyOrField(before, parts.First());
            var isEnumerable = QueryableHelpers.IsGenericEnumerable(currentExpression);

            // allow interaction through callback.
            var alteredExpression = isEnumerable ? simple(before, currentExpression, isFirst, isLast) : collection(before, currentExpression, isFirst, isLast);

            // if its last we are done :)
            if (isLast)
                return alteredExpression;

            // not enumerable pretty simple.
            if (!isEnumerable)
                return InternalNavigatePath(alteredExpression, parts.Skip(1).ToList(), simple, collection, collectionHandling);

            // enumerable.
            var listGenericArgumentType = alteredExpression.Type.GetGenericArguments().First();

            // sub param.
            var innerParameter = Expression.Parameter(listGenericArgumentType);
            var innerExpr = InternalNavigatePath(innerParameter, parts.Skip(1).ToList(), simple, collection, collectionHandling);
            var lambda = Expression.Lambda(innerExpr, innerParameter);
            var collectionHandlingResult = collectionHandling(alteredExpression, innerExpr, lambda);
            return collectionHandlingResult;
        }

        public static Expression NavigatePath(ParameterExpression param, string path,
            SimplePathNavigated simple,
            CollectionPathNavigated collection,
             CollectionHandling collectionHandling)
        {
            var parts = path.Split('.').ToList();
            return InternalNavigatePath(param, parts, simple, collection, collectionHandling);
        }
    }
}
