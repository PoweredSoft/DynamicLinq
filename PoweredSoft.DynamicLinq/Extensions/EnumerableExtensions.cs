using PoweredSoft.DynamicLinq.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoweredSoft.DynamicLinq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> list, string path, ConditionOperators conditionOperator, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField,
            QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => list.AsQueryable().Where(path, conditionOperator, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public static IEnumerable<T> Where<T>(this IEnumerable<T> list, Action<QueryBuilder<T>> callback)
            => list.Query(callback);

        public static IEnumerable<T> Query<T>(this IEnumerable<T> list, Action<QueryBuilder<T>> callback)
            => list.AsQueryable().Query(callback);

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> list, string path, QuerySortDirection sortDirection, bool appendSort)
            => list.AsQueryable().Sort(path, sortDirection, appendSort);

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string path)
            => list.AsQueryable().OrderBy(path);

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> list, string path)
            => list.AsQueryable().OrderByDescending(path);

        public static IEnumerable<T> ThenBy<T>(this IEnumerable<T> list, string path)
            => list.AsQueryable().ThenBy(path);

        public static IEnumerable<T> ThenByDescending<T>(this IEnumerable<T> list, string path)
            => list.AsQueryable().ThenByDescending(path);
    }
}
