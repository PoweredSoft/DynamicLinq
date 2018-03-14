using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public abstract partial class WhereBuilderBase
    {
        public bool IsNullCheckingEnabled { get; protected set; } = false;

        public List<WhereBuilderCondition> Filters { get; protected set; } = new List<WhereBuilderCondition>();

        public virtual WhereBuilderBase NullChecking(bool check = true)
        {
            IsNullCheckingEnabled = check;
            return this;
        }

        public virtual WhereBuilderBase Compare(string path, ConditionOperators conditionOperators, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField,
            bool and = true, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
        {
            Filters.Add(new WhereBuilderCondition
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

        protected abstract WhereBuilderBase GetSubQueryBuilder();

        public virtual WhereBuilderBase SubQuery(Action<WhereBuilderBase> subQuery, bool and = true)
        {
            // create query builder for same type.
            var qb = GetSubQueryBuilder();
            qb.NullChecking(IsNullCheckingEnabled);

            // callback.
            subQuery(qb);

            // create a query part.
            var part = new WhereBuilderCondition();
            part.And = and;
            part.Conditions = qb.Filters;
            Filters.Add(part);

            //return self.
            return this;
        }        
    }
}
