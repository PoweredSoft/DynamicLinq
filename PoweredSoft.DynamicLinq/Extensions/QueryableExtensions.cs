using PoweredSoft.DynamicLinq.Fluent;
using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> query, string path, ConditionOperators conditionOperator, object value, 
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField)
        {
            query = query.Query(qb => qb.Compare(path, conditionOperator, value, convertStrategy: convertStrategy));
            return query;
        }
        
        public static IQueryable<T> Query<T> (this IQueryable<T> query, Action<QueryBuilder<T>> callback)
        {
            var queryBuilder = new QueryBuilder<T>(query);
            callback(queryBuilder);
            var ret = queryBuilder.Build();
            return ret;
        }
    }
}
