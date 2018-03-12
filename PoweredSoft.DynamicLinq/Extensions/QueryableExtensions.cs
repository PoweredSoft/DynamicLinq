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

        public static IQueryable<T> Where<T>(this IQueryable<T> query, Action<QueryBuilder<T>> callback)
            => query.Query(callback);

        public static IQueryable<T> Query<T> (this IQueryable<T> query, Action<QueryBuilder<T>> callback)
        {
            var queryBuilder = new QueryBuilder<T>(query);
            callback(queryBuilder);
            var ret = queryBuilder.Build();
            return ret;
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, string path, QuerySortDirection sortDirection, bool appendSort)
        {
            var qb = new QueryBuilder<T>(query);
            qb.Sort(path, sortDirection, appendSort);
            var ret = qb.Build();
            return ret;
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string path)
        {
            var qb = new QueryBuilder<T>(query);
            qb.OrderBy(path);
            var ret = qb.Build();
            return ret;
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string path)
        {
            var qb = new QueryBuilder<T>(query);
            qb.OrderByDescending(path);
            var ret = qb.Build();
            return ret;
        }

        public static IQueryable<T> ThenBy<T>(this IQueryable<T> query, string path)
        {
            var qb = new QueryBuilder<T>(query);
            qb.ThenBy(path);
            var ret = qb.Build();
            return ret;
        }

        public static IQueryable<T> ThenByDescending<T>(this IQueryable<T> query, string path)
        {
            var qb = new QueryBuilder<T>(query);
            qb.ThenByDescending(path);
            var ret = qb.Build();
            return ret;
        }

        public static IQueryable GroupBy<T>(this IQueryable<T> query, string path)
            => QueryableHelpers.GroupBy(query, typeof(T), path);

        public static IQueryable GroupBy(this IQueryable query, Type type, string path)
            => QueryableHelpers.GroupBy(query, type, path);

        public static IQueryable GroupBy<T>(this IQueryable<T> query, Action<GroupBuilder> callback)
            => query.GroupBy(typeof(T), callback);

        public static IQueryable GroupBy(this IQueryable query, Type type, Action<GroupBuilder> callback)
        {
            var groupBuilder = new GroupBuilder();
            callback(groupBuilder);
            if (groupBuilder.Empty)
                throw new Exception("No group specified, please specify at least one group");

            return QueryableHelpers.GroupBy(query, type, groupBuilder.Parts, groupBuilder.Type, groupBuilder.EqualityComparerType);
        }
    }
}
