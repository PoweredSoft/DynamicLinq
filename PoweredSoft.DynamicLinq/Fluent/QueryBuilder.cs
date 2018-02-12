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
            part.Parts = qb.Filters;
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

            var parameter = Expression.Parameter(typeof(T), "t");
            var expression = BuildFilterExpression(parameter, Filters);
            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            query = query.Where(lambda);
            return query;
        }

        protected virtual Expression BuildFilterExpression(ParameterExpression parameter, List<QueryBuilderFilter> filters)
        {
            Expression ret = null;

            filters.ForEach(filter =>
            {
                Expression innerExpression;
                if (filter.Parts?.Any() == true)
                    innerExpression = BuildFilterExpression(parameter, filter.Parts);
                else
                    innerExpression = BuildFilterExpression(parameter, filter);

                if (ret != null)
                    ret = filter.And ? Expression.And(ret, innerExpression) : Expression.Or(ret, innerExpression);
                else
                    ret = innerExpression;
            });

            return ret;
        }

        protected virtual Expression BuildFilterExpression(ParameterExpression parameter, QueryBuilderFilter filter)
        {
            var member = QueryableHelpers.ResolvePathForExpression(parameter, filter.Path);
            var constant = QueryableHelpers.ResolveConstant(member, filter.Value, filter.ConvertStrategy);
            var expression = QueryableHelpers.GetConditionExpressionForMember(parameter, member, filter.ConditionOperator, constant);
            return expression;
        }
    }
}
