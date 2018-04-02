using PoweredSoft.DynamicLinq.Fluent;
using System;
using System.Collections;
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

        public static IEnumerable<T> Where<T>(this IEnumerable<T> list, Action<WhereBuilder> callback)
            => list.Query(callback);

        public static IEnumerable<T> Query<T>(this IEnumerable<T> list, Action<WhereBuilder> callback)
            => list.AsQueryable().Query(callback);

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> list, string path, QueryOrderByDirection sortDirection, bool appendSort)
            => list.AsQueryable().OrderBy(path, sortDirection, appendSort);

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, string path)
            => list.AsQueryable().OrderBy(path);

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> list, string path)
            => list.AsQueryable().OrderByDescending(path);

        public static IEnumerable<T> ThenBy<T>(this IEnumerable<T> list, string path)
            => list.AsQueryable().ThenBy(path);

        public static IEnumerable<T> ThenByDescending<T>(this IEnumerable<T> list, string path)
            => list.AsQueryable().ThenByDescending(path);

        public static IQueryable GroupBy<T>(this IEnumerable<T> list, string path)
            => list.AsQueryable().GroupBy(typeof(T), path);

        public static IQueryable GroupBy(this IEnumerable list, Type type, string path)
            => list.AsQueryable().GroupBy(type, path);

        public static IQueryable GroupBy<T>(this IEnumerable<T> list, Action<GroupBuilder> callback)
            => list.AsQueryable().GroupBy(typeof(T), callback);

        public static IQueryable GroupBy(this IEnumerable list, Type type, Action<GroupBuilder> callback)
            => list.AsQueryable().GroupBy(type, callback);

        public static List<T> Reversed<T>(this List<T> list)
        {
            var copy = list.ToList();
            copy.Reverse();
            return copy;
        }

        public delegate void ForEachDelegate<T>(T element, int index);

        public static void ForEach<T>(this List<T> list, ForEachDelegate<T> callback)
        {
            for (var i = 0; i < list.Count; i++)
                callback(list[i], i);
        }

        public static void ReversedForEach<T>(this List<T> list, Action<T> action)
        {
            list.Reversed().ForEach(action);
        }

        public static void ReversedForEach<T>(this List<T> list, ForEachDelegate<T> callback)
        {
            var reversed = list.Reversed();
            reversed.ForEach(callback);
        }


    }
}
