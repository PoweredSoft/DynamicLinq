using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class QueryBuilder<T> : QueryBuilderBase
    {
        public IQueryable<T> Query { get; set; }

        public Type QueryableType { get; set; }
       
        public QueryBuilder(IQueryable<T> query)
        {
            Query = query;
        }

        public virtual IQueryable<T> Build()
        {
            // the query.
            var query = Query;

            // build the filters.
            query = BuildFilters(query);

            // build the sorts
            query = BuildSorts(query);
            
            return query;
        }

        protected virtual IQueryable<T> BuildSorts(IQueryable<T> query)
        {
            Sorts.ForEach(sort =>
            {
                query = QueryableHelpers.CreateSortExpression(query, sort.Path, sort.sortDirection, sort.AppendSort);
            });

            return query;
        }

        protected virtual IQueryable<T> BuildFilters(IQueryable<T> query)
        {
            if (Filters == null || Filters?.Count() == 0)
                return query;

            // shared parameter.
            var sharedParameter = Expression.Parameter(typeof(T), "t");

            // build the expression.
            var filterExpressionMerged = BuildFilterExpression(sharedParameter, Filters);
            
            // make changes on the query.
            query = query.Where(filterExpressionMerged);

            return query;
        }

        protected virtual Expression<Func<T, bool>> BuildFilterExpression(ParameterExpression parameter, List<QueryBuilderFilter> filters)
        {
            Expression<Func<T, bool>> temp = null;

            

            filters.ForEach(filter =>
            {
                Expression<Func<T, bool>> innerExpression;
                if (filter.Filters?.Any() == true)
                    innerExpression = BuildFilterExpression(parameter, filter.Filters);
                else
                    innerExpression = BuildFilterExpression(parameter, filter);

                if (temp == null)
                {
                    temp = innerExpression;
                }
                else
                {
                    if (filter.And)
                        temp = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(temp.Body, innerExpression.Body), parameter);
                    else
                        temp = Expression.Lambda<Func<T, bool>>(Expression.OrElse(temp.Body, innerExpression.Body), parameter);
                }
                    
            });

            return temp;
        }

        protected virtual Expression<Func<T, bool>> BuildFilterExpression(ParameterExpression parameter, QueryBuilderFilter filter)
        {
            var ret = QueryableHelpers.CreateFilterExpression<T>(
                filter.Path,
                filter.ConditionOperator,
                filter.Value,
                filter.ConvertStrategy,
                filter.CollectionHandling,
                parameter: parameter,
                nullChecking: IsNullCheckingEnabled
            );

            return ret;
        }

        protected override QueryBuilderBase GetSubQueryBuilder()
        {
            return new QueryBuilder<T>(Query);
        }
    }
}
