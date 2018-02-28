using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public abstract partial class QueryBuilderBase
    {
        public bool IsNullCheckingEnabled { get; protected set; } = false;

        public List<QueryBuilderFilter> Filters { get; protected set; } = new List<QueryBuilderFilter>();

        public List<QueryBuilderSort> Sorts { get; protected set; } = new List<QueryBuilderSort>();

        public virtual QueryBuilderBase NullChecking(bool check = true)
        {
            IsNullCheckingEnabled = check;
            return this;
        }

        public virtual QueryBuilderBase Compare(string path, ConditionOperators conditionOperators, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField,
            bool and = true, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
        {
            Filters.Add(new QueryBuilderFilter
            {
                And = and,
                ConditionOperator = conditionOperators,
                Path = path,
                Value = value,
                ConvertStrategy = convertStrategy,
                CollectionHandling = collectionHandling
            });

            return this;
        }

        public virtual QueryBuilderBase Sort(string path, QuerySortDirection sortDirection, bool appendSort)
        {
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                sortDirection = sortDirection,
                AppendSort = appendSort
            });
            return this;
        }

        protected abstract QueryBuilderBase GetSubQueryBuilder();

        public virtual QueryBuilderBase SubQuery(Action<QueryBuilderBase> subQuery, bool and = true)
        {
            // create query builder for same type.
            var qb = GetSubQueryBuilder();
            qb.NullChecking(IsNullCheckingEnabled);

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

        public virtual QueryBuilderBase OrderBy(string path)
        {
            Sorts.Clear();
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                sortDirection = QuerySortDirection.Ascending,
                AppendSort = false
            });
            return this;
        }

        public virtual QueryBuilderBase OrderByDescending(string path)
        {
            Sorts.Clear();
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                sortDirection = QuerySortDirection.Descending,
                AppendSort = false
            });
            return this;
        }

        public virtual QueryBuilderBase ThenBy(string path)
        {
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                sortDirection = QuerySortDirection.Ascending,
                AppendSort = true
            });
            return this;
        }

        public virtual QueryBuilderBase ThenByDescending(string path)
        {
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                sortDirection = QuerySortDirection.Descending,
                AppendSort = true
            });
            return this;
        }
    }
}
