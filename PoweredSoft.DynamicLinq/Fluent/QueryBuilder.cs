using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class QueryBuilder<T>
    {
        public IQueryable<T> Query { get; set; }

        public Type QueryableType { get; set; }

        public List<QueryBuilderFilter> Filters { get; protected set; } = new List<QueryBuilderFilter>();

        public List<QueryBuilderSort> Sorts { get; protected set; } = new List<QueryBuilderSort>();

        public QueryBuilder(IQueryable<T> query)
        {
            Query = query;
        }

        public virtual QueryBuilder<T> Compare(string path, ConditionOperators conditionOperators, object value, 
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, 
            bool and = true)
        {
            Filters.Add(new QueryBuilderFilter
            {
                And = and,
                ConditionOperator = conditionOperators,
                Path = path,
                Value = value,
                ConvertStrategy = convertStrategy
            });

            return this;
        }

        public virtual QueryBuilder<T> Sort(string path, SortOrder sortOrder, bool appendSort)
        {
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                SortOrder = sortOrder,
                AppendSort = appendSort
            });
            return this;
        }

        public virtual QueryBuilder<T> SubQuery(Action<QueryBuilder<T>> subQuery, bool and = true)
        {
            // create query builder for same type.
            var qb = new QueryBuilder<T>(Query);

            // callback.
            subQuery(qb);
            
            // create a query part.
            var part = new QueryBuilderFilter();
            part.And = and;
            part.Filters = qb.Filters;
            Filters.Add(part);
            
            //return self.
            return this;
        }

        public QueryBuilder<T> And(string path, ConditionOperators conditionOperator, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField)
            => Compare(path, conditionOperator, value, convertStrategy: convertStrategy, and: true);

        public QueryBuilder<T> Or(string path, ConditionOperators conditionOperator, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField)
            => Compare(path, conditionOperator, value, convertStrategy: convertStrategy, and: false);

        public QueryBuilder<T> And(Action<QueryBuilder<T>> subQuery)
            => SubQuery(subQuery, true);

        public QueryBuilder<T> Or(Action<QueryBuilder<T>> subQuery)
            => SubQuery(subQuery, false);

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

        public virtual QueryBuilder<T> OrderBy(string path)
        {
            Sorts.Clear();
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                SortOrder = SortOrder.Ascending,
                AppendSort = false
            });
            return this;
        }

        public virtual QueryBuilder<T> OrderByDescending(string path)
        {
            Sorts.Clear();
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                SortOrder = SortOrder.Descending,
                AppendSort = false
            });
            return this;
        }

        public virtual QueryBuilder<T> ThenBy(string path)
        {
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                SortOrder = SortOrder.Ascending,
                AppendSort = true
            });
            return this;
        }

        public virtual QueryBuilder<T> ThenByDescending(string path)
        {
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                SortOrder = SortOrder.Descending,
                AppendSort = true
            });
            return this;
        }

        protected virtual IQueryable<T> BuildSorts(IQueryable<T> query)
        {
            Sorts.ForEach(sort =>
            {
                query = QueryableHelpers.CreateSortExpression(query, sort.Path, sort.SortOrder, sort.AppendSort);
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
                        temp = Expression.Lambda<Func<T, bool>>(Expression.And(temp.Body, innerExpression.Body), parameter);
                    else
                        temp = Expression.Lambda<Func<T, bool>>(Expression.Or(temp.Body, innerExpression.Body), parameter);
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
                parameter: parameter
            );

            return ret;
        }
    }
}
