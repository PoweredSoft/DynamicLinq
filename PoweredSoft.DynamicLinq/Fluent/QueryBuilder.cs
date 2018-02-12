using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
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

        public List<QueryFilterPart> Parts { get; protected set; } = new List<QueryFilterPart>();

        public QueryBuilder(IQueryable<T> query)
        {
            Query = query;
        }

        public virtual QueryBuilder<T> Compare(string path, ConditionOperators conditionOperators, object value, 
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, 
            bool and = true)
        {
            Parts.Add(new QueryFilterPart
            {
                And = and,
                ConditionOperator = conditionOperators,
                Path = path,
                Value = value,
                ConvertStrategy = convertStrategy
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
            var part = new QueryFilterPart();
            part.And = and;
            part.Parts = qb.Parts;
            Parts.Add(part);
            
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

            // execute the filters.
            query = BuildFilters(query);

            
            return query;
        }

        protected virtual IQueryable<T> BuildFilters(IQueryable<T> query)
        {
            var parameter = Expression.Parameter(typeof(T), "t");
            var expression = BuildFilterExpression(parameter, Parts);
            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            query = query.Where(lambda);
            return query;
        }

        protected virtual Expression BuildFilterExpression(ParameterExpression parameter, List<QueryFilterPart> parts)
        {
            Expression ret = null;

            parts.ForEach(part =>
            {
                Expression innerExpression;
                if (part.Parts?.Any() == true)
                    innerExpression = BuildFilterExpression(parameter, part.Parts);
                else
                    innerExpression = BuildFilterExpression(parameter, part);

                if (ret != null)
                    ret = part.And ? Expression.And(ret, innerExpression) : Expression.Or(ret, innerExpression);
                else
                    ret = innerExpression;
            });

            return ret;
        }

        protected virtual Expression BuildFilterExpression(ParameterExpression parameter, QueryFilterPart part)
        {
            var member = QueryableHelpers.ResolvePathForExpression(parameter, part.Path);
            var constant = QueryableHelpers.ResolveConstant(member, part.Value, part.ConvertStrategy);
            var expression = QueryableHelpers.GetConditionExpressionForMember(parameter, member, part.ConditionOperator, constant);
            return expression;
        }
    }
}
