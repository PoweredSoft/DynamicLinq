using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public abstract class QueryBuilderBase
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

        public virtual QueryBuilderBase Sort(string path, SortOrder sortOrder, bool appendSort)
        {
            Sorts.Add(new QueryBuilderSort
            {
                Path = path,
                SortOrder = sortOrder,
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

        public QueryBuilderBase And(string path, ConditionOperators conditionOperator, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Compare(path, conditionOperator, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, and: true);

        public QueryBuilderBase Or(string path, ConditionOperators conditionOperator, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Compare(path, conditionOperator, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, and: false);

        public QueryBuilderBase And(Action<QueryBuilderBase> subQuery)
            => SubQuery(subQuery, true);

        public QueryBuilderBase Or(Action<QueryBuilderBase> subQuery)
            => SubQuery(subQuery, false);
    }
}
