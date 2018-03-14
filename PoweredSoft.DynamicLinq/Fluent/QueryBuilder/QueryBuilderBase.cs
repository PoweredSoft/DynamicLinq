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

        public virtual QueryBuilderBase NullChecking(bool check = true)
        {
            IsNullCheckingEnabled = check;
            return this;
        }

        public virtual QueryBuilderBase Compare(string path, ConditionOperators conditionOperators, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField,
            bool and = true, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
        {
            Filters.Add(new QueryBuilderFilter
            {
                And = and,
                ConditionOperator = conditionOperators,
                Path = path,
                Value = value,
                ConvertStrategy = convertStrategy,
                CollectionHandling = collectionHandling,
                StringComparisation = stringComparision
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
    }
}
