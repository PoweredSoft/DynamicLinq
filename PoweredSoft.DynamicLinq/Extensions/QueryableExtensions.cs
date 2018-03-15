using PoweredSoft.DynamicLinq.Fluent;
using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, string path, ConditionOperators conditionOperator, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField,
            QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
        {
            query = query.Query(qb => qb.Compare(path, conditionOperator, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision));
            return query;
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> query, Action<WhereBuilder> callback)
            => query.Query(callback);

        public static IQueryable<T> Query<T>(this IQueryable<T> query, Action<WhereBuilder> callback)
        {
            var queryBuilder = new WhereBuilder(query);
            callback(queryBuilder);
            var ret = queryBuilder.Build();
            return (IQueryable<T>)ret;
        }

        // generic.
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string path, QueryOrderByDirection direction, bool append)
        {
            IQueryable queryable = query;
            query = queryable.OrderBy(path, direction, append) as IQueryable<T>;
            return query;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string path)
            => query.OrderBy(path, QueryOrderByDirection.Ascending, false);

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string path)
            => query.OrderBy(path, QueryOrderByDirection.Descending, false);

        public static IQueryable<T> ThenBy<T>(this IQueryable<T> query, string path)
            => query.OrderBy(path, QueryOrderByDirection.Ascending, true);

        public static IQueryable<T> ThenByDescending<T>(this IQueryable<T> query, string path)
            => query.OrderBy(path, QueryOrderByDirection.Descending, true);

        // non generic.
        public static IQueryable OrderBy(this IQueryable query, string path, QueryOrderByDirection direction, bool append)
        {
            var qb = new OrderByBuilder(query);
            qb.OrderBy(path, direction, append);
            var ret = qb.Build();
            return ret;
        }

        public static IQueryable OrderBy(this IQueryable query, string path)
            => query.OrderBy(path, QueryOrderByDirection.Ascending, false);

        public static IQueryable OrderByDescending(this IQueryable query, string path)
            => query.OrderBy(path, QueryOrderByDirection.Descending, false);

        public static IQueryable ThenBy(this IQueryable query, string path)
            => query.OrderBy(path, QueryOrderByDirection.Ascending, true);

        public static IQueryable ThenByDescending(this IQueryable query, string path)
            => query.OrderBy(path, QueryOrderByDirection.Descending, true);

        // group by
        public static IQueryable GroupBy<T>(this IQueryable<T> query, string path)
            => QueryableHelpers.GroupBy(query, typeof(T), path);

        public static IQueryable GroupBy(this IQueryable query, Type type, string path)
            => QueryableHelpers.GroupBy(query, type, path);

        public static IQueryable GroupBy<T>(this IQueryable<T> query, Action<GroupBuilder> callback)
            => query.GroupBy(typeof(T), callback);

        public static IQueryable GroupBy(this IQueryable query, Type type, Action<GroupBuilder> callback)
        {
            var groupBuilder = new GroupBuilder(query);
            callback(groupBuilder);
            var ret = groupBuilder.Build();
            return ret;
        }

        public static IQueryable Select(this IQueryable query, Action<SelectBuilder> callback)
        {
            var sb = new SelectBuilder(query);
            callback(sb);
            var ret = sb.Build();
            return ret;
        }

        public static List<object> ToObjectList(this IQueryable query)
        {
            // Expression call tolist?
            var ret = new List<object>();
            foreach (var o in query)
                ret.Add(o);

            return ret;
        }

        public static List<dynamic> ToDynamicList(this IQueryable query)
        {
            var ret = new List<dynamic>();
            foreach (var o in query)
                ret.Add(o);

            return ret;
        }
        public static List<DynamicClass> ToDynamicClassList(this IQueryable query)
        {
            if (!typeof(DynamicClass).IsAssignableFrom(query.ElementType))
                throw new Exception($"{query.ElementType} does not inherit DynamicClass");

            var ret = query.Cast<DynamicClass>().ToList();
            return ret;
        }
    }
}
