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

        public QueryBuilder<T> Compare(string path, ConditionOperators conditionOperators, object value, 
            bool convertConstantToLeftOperator = true, bool and = true)
        {
            Parts.Add(new QueryFilterPart
            {
                And = and,
                ConditionOperator = conditionOperators,
                Path = path,
                Value = value,
                ConvertConstantToLeftOperator = convertConstantToLeftOperator
            });

            return this;
        }

        public QueryBuilder<T> SubQuery(Action<QueryBuilder<T>> subQuery, bool and = true)
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

        public QueryBuilder<T> And(string path, ConditionOperators conditionOperator, object value, bool convertConstantToLeftOperator = true)
            => Compare(path, conditionOperator, value, convertConstantToLeftOperator: convertConstantToLeftOperator, and: true);

        public QueryBuilder<T> Or(string path, ConditionOperators conditionOperator, object value, bool convertConstantToLeftOperator = true)
            => Compare(path, conditionOperator, value, convertConstantToLeftOperator: convertConstantToLeftOperator, and: false);

        public QueryBuilder<T> And(Action<QueryBuilder<T>> subQuery)
            => SubQuery(subQuery, true);

        public QueryBuilder<T> Or(Action<QueryBuilder<T>> subQuery)
            => SubQuery(subQuery, false);

        public IQueryable<T> Build()
        {
            var parameter = Expression.Parameter(typeof(T), "t");
            var expression = BuildExpression(parameter, Parts);
            var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            var query = Query.Where(lambda);
            return query;
        }

        protected Expression BuildExpression(ParameterExpression parameter, List<QueryFilterPart> parts)
        {
            Expression ret = null;

            parts.ForEach(part =>
            {
                Expression innerExpression;
                if (part.Parts?.Any() == true)
                    innerExpression = BuildExpression(parameter, part.Parts);
                else
                    innerExpression = BuildExpression(parameter, part);

                if (ret != null)
                    ret = part.And ? Expression.And(ret, innerExpression) : Expression.Or(ret, innerExpression);
                else
                    ret = innerExpression;
            });

            return ret;
        }

        private Expression BuildExpression(ParameterExpression parameter, QueryFilterPart part)
        {
            var member = QueryableHelpers.ResolvePathForExpression(parameter, part.Path);
            var constant = part.ConvertConstantToLeftOperator ? QueryableHelpers.GetConstantSameAsLeftOperator(member, part.Value) : Expression.Constant(part.Value);
            var expression = QueryableHelpers.GetConditionExpressionForMember(parameter, member, part.ConditionOperator, constant);
            return expression;
        }
    }
}
