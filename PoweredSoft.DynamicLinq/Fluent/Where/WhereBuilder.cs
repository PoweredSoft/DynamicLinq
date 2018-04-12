using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public partial class WhereBuilder : IQueryBuilder
    {
        public IQueryable Query { get; set; }
        public Type QueryableType { get; set; }
        public List<WhereBuilderCondition> Filters { get; protected set; } = new List<WhereBuilderCondition>();

        public WhereBuilder(IQueryable query)
        {
            Query = query;
            QueryableType = query.ElementType;
        }

        public bool IsNullCheckingEnabled { get; protected set; } = false;

        public virtual WhereBuilder NullChecking(bool check = true)
        {
            IsNullCheckingEnabled = check;
            return this;
        }

        public virtual WhereBuilder Compare(string path, ConditionOperators conditionOperators, object value,
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

        public virtual WhereBuilder SubQuery(Action<WhereBuilder> subQuery, bool and = true)
        {
            // create query builder for same type.
            var qb = new WhereBuilder(Query);
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

        public virtual IQueryable Build()
        {
            // the query.
            var query = Query;

            if (Filters == null || Filters?.Count() == 0)
                return query;

            // shared parameter.
            var sharedParameter = Expression.Parameter(QueryableType, "t");

            // build the expression.
            var filterExpressionMerged = BuildConditionExpression(sharedParameter, Filters);

            // create the where expression.
            var whereExpression = Expression.Call(typeof(Queryable), "Where", new[] { query.ElementType }, query.Expression, filterExpressionMerged);

            // lets see what happens here.
            query = query.Provider.CreateQuery(whereExpression);

            return query;
        }

        protected virtual Expression BuildConditionExpression(ParameterExpression parameter, List<WhereBuilderCondition> filters)
        {
            Expression temp = null;

            filters.ForEach(filter =>
            {
                Expression innerExpression;
                if (filter.Conditions?.Any() == true)
                    innerExpression = BuildConditionExpression(parameter, filter.Conditions);
                else
                    innerExpression = BuildConditionExpression(parameter, filter);

                if (temp == null)
                {
                    temp = innerExpression;
                }
                else
                {
                    var body = ((LambdaExpression)temp).Body;
                    var innerEpressionBody = ((LambdaExpression)innerExpression).Body;

                    if (filter.And)
                        temp = Expression.Lambda(Expression.AndAlso(body, innerEpressionBody), parameter);
                    else
                        temp = Expression.Lambda(Expression.OrElse(body, innerEpressionBody), parameter);
                }
                    
            });

            return temp;
        }

        protected virtual Expression BuildConditionExpression(ParameterExpression parameter, WhereBuilderCondition filter)
        {
            var ret = QueryableHelpers.CreateConditionExpression(
                parameter.Type,
                filter.Path,
                filter.ConditionOperator,
                filter.Value,
                filter.ConvertStrategy,
                filter.CollectionHandling,
                parameter: parameter,
                nullChecking: IsNullCheckingEnabled,
                stringComparision: filter.StringComparisation
            );

            return ret;
        }
    }
}
