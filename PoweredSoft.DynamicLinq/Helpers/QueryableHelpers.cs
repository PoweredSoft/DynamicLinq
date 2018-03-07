using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Helpers
{
    public static class QueryableHelpers
    {
        public static Expression GetConditionExpressionForMember(ParameterExpression parameter, Expression member, ConditionOperators conditionOperator, ConstantExpression constant, StringComparison? stringComparision)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (member == null)
                throw new ArgumentNullException("member");

            if (constant == null)
                throw new ArgumentNullException("constant");


            Type stringType = typeof(string);

            Expression ret = null;

            if (conditionOperator == ConditionOperators.Equal)
            {
                if (member.Type == stringType && stringComparision.HasValue)
                    ret = Expression.Call(member, Constants.StringEqualWithComparisation, constant, Expression.Constant(stringComparision.Value));
                else
                    ret = Expression.Equal(member, constant);
            }
            else if (conditionOperator == ConditionOperators.NotEqual)
            {
                if (member.Type == stringType && stringComparision.HasValue)
                    ret = Expression.Not(Expression.Call(member, Constants.StringEqualWithComparisation, constant, Expression.Constant(stringComparision.Value)));
                else
                    ret = Expression.NotEqual(member, constant);
            }
            else if (conditionOperator == ConditionOperators.GreaterThan)
                ret = Expression.GreaterThan(member, constant);
            else if (conditionOperator == ConditionOperators.GreaterThanOrEqual)
                ret = Expression.GreaterThanOrEqual(member, constant);
            else if (conditionOperator == ConditionOperators.LessThan)
                ret = Expression.LessThan(member, constant);
            else if (conditionOperator == ConditionOperators.LessThanOrEqual)
                ret = Expression.LessThanOrEqual(member, constant);
            else if (conditionOperator == ConditionOperators.Contains)
            {
                if (member.Type == stringType && stringComparision.HasValue)
                    ret = Expression.GreaterThan(Expression.Call(member, Constants.IndexOfMethod, constant, Expression.Constant(stringComparision.Value)), Expression.Constant(-1));
                else
                    ret = Expression.Call(member, Constants.ContainsMethod, constant);
            }
            else if (conditionOperator == ConditionOperators.StartsWith)
            {
                if (member.Type == stringType && stringComparision.HasValue)
                    ret = Expression.Call(member, Constants.StartsWithMethodWithComparisation, constant, Expression.Constant(stringComparision.Value));
                else
                    ret = Expression.Call(member, Constants.StartsWithMethod, constant);
            }
            else if (conditionOperator == ConditionOperators.EndsWith)
            {
                if (member.Type == stringType && stringComparision.HasValue)
                    ret = Expression.Call(member, Constants.EndsWithMethodWithComparisation, constant, Expression.Constant(stringComparision.Value));
                else
                    ret = Expression.Call(member, Constants.EndsWithMethod, constant);
            }
            else
                throw new ArgumentException("conditionOperator", "Must supply a known condition operator");

            return ret;            
        }

        /// <summary>
        /// Returns the right expression for a path supplied.
        /// </summary>
        /// <param name="param">Expression.Parameter(typeOfClassOrInterface)</param>
        /// <param name="path">the path you wish to resolve example Contact.Profile.FirstName</param>
        /// <returns></returns>
        public static Expression ResolvePathForExpression(ParameterExpression param, string path)
        {
            Expression body = param;
            foreach (var member in path.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }
            return body;
        }

        public static ConstantExpression GetConstantSameAsLeftOperator(Expression member, object value)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            if (value == null)
                return Expression.Constant(null);

            // the types.
            var valueType = value.GetType();
            var memberType = member.Type;

            // if match.
            if (valueType == memberType)
                return Expression.Constant(value);

            // attempt a conversion.
            object convertedValue = TypeHelpers.ConvertFrom(memberType, value);
            return Expression.Constant(convertedValue, memberType);
        }

        public static ConstantExpression ResolveConstant(Expression member, object value, QueryConvertStrategy convertStrategy)
        {
            if (convertStrategy == QueryConvertStrategy.LeaveAsIs)
                return Expression.Constant(value);

            if (convertStrategy == QueryConvertStrategy.SpecifyType)
                return Expression.Constant(value, member.Type);

            if (convertStrategy == QueryConvertStrategy.ConvertConstantToComparedPropertyOrField)
                return QueryableHelpers.GetConstantSameAsLeftOperator(member, value);

            throw new NotSupportedException($"{convertStrategy} supplied is not recognized");
        }

        public static IQueryable<T> CreateSortExpression<T>(IQueryable<T> query, string sortPath, QuerySortDirection sortDirection, bool appendSort = true)
        {
            var parameter = Expression.Parameter(typeof(T), "t");
            var member = QueryableHelpers.ResolvePathForExpression(parameter, sortPath);

            string sortCommand = null;
            if (sortDirection == QuerySortDirection.Descending)
                sortCommand = appendSort == false ? "OrderByDescending" : "ThenByDescending";
            else
                sortCommand = appendSort == false ? "OrderBy" : "ThenBy";

            var expression = Expression.Lambda(member, parameter);

            var resultExpression = Expression.Call
                    (typeof(Queryable),
                    sortCommand,
                    new Type[] { typeof(T), member.Type },
                    query.Expression,
                    Expression.Quote(expression)
                );

            query = query.Provider.CreateQuery<T>(resultExpression);
            return query;
        }

        internal static Expression InternalCreateFilterExpression(int recursionStep, Type type, ParameterExpression parameter, Expression current, List<string> parts,
            ConditionOperators condition, object value, QueryConvertStrategy convertStrategy, QueryCollectionHandling collectionHandling, bool nullChecking, StringComparison? stringComparison)
        {
            var partStr = parts.First();
            var isLast = parts.Count == 1;

            // the member expression.
            var memberExpression = Expression.PropertyOrField(current, partStr);

            // TODO : maybe support that last part is collection but what do we do?
            // not supported yet.
            if (isLast && IsEnumerable(memberExpression) && value != null)
                throw new NotSupportedException("Can only compare collection to null");


            // create the expression and return it.
            if (isLast)
            {
                var constant = QueryableHelpers.ResolveConstant(memberExpression, value, convertStrategy);
                var filterExpression = QueryableHelpers.GetConditionExpressionForMember(parameter, memberExpression, condition, constant, stringComparison);
                var lambda = Expression.Lambda(filterExpression, parameter);
                return lambda;
            }

            // null check.
            Expression nullCheckExpression = null;
            if (nullChecking)
                nullCheckExpression = Expression.NotEqual(memberExpression, Expression.Constant(null));

            if (IsEnumerable(memberExpression))
            {
                var listGenericArgumentType = memberExpression.Type.GetGenericArguments().First();
                var innerParameter = Expression.Parameter(listGenericArgumentType, $"t{++recursionStep}");
                var innerLambda = InternalCreateFilterExpression(recursionStep, listGenericArgumentType, innerParameter, innerParameter, parts.Skip(1).ToList(), condition, value, convertStrategy, collectionHandling, nullChecking, stringComparison);

                // the collection method.
                var collectionMethod = GetCollectionMethod(collectionHandling);
                var genericMethod = collectionMethod.MakeGenericMethod(listGenericArgumentType);
                var callResult = Expression.Call(genericMethod, memberExpression, innerLambda);

                if (nullCheckExpression != null)
                {
                    var nullCheckResult = Expression.AndAlso(nullCheckExpression, callResult);
                    return Expression.Lambda(nullCheckResult, parameter);
                }

                return Expression.Lambda(callResult, parameter);
            }
            else
            {
                if (nullCheckExpression != null)
                {
                    var pathExpr = InternalCreateFilterExpression(recursionStep, type, parameter, memberExpression, parts.Skip(1).ToList(), condition, value, convertStrategy, collectionHandling, nullChecking, stringComparison);
                    var nullCheckResult = Expression.AndAlso(nullCheckExpression, pathExpr);
                    return nullCheckResult;
                }

                return InternalCreateFilterExpression(recursionStep, type, parameter, memberExpression, parts.Skip(1).ToList(), condition, value, convertStrategy, collectionHandling, nullChecking, stringComparison);
            }
        }

        public static MethodInfo GetCollectionMethod(QueryCollectionHandling collectionHandling)
        {
            if (collectionHandling == QueryCollectionHandling.All)
                return Constants.AllMethod;
            else if (collectionHandling == QueryCollectionHandling.Any)
                return Constants.AnyMethod;

            throw new NotSupportedException($"{collectionHandling} is not supported");
        }

        public static Expression<Func<T, bool>> CreateFilterExpression<T>(string path, 
            ConditionOperators condition, 
            object value, 
            QueryConvertStrategy convertStrategy, 
            QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any,
            ParameterExpression parameter = null,
            bool nullChecking = false,
            StringComparison? stringComparision = null)
        {
            if (parameter == null)
                parameter = Expression.Parameter(typeof(T), "t");

            var parts = path.Split('.').ToList();
            var result = InternalCreateFilterExpression(1, typeof(T), parameter, parameter, parts, condition, value, convertStrategy, collectionHandling, nullChecking, stringComparision);
            var ret = result as Expression<Func<T, bool>>;
            return ret;
        }

        public static bool IsEnumerable(MemberExpression member)
        {
            var ret = member.Type.FullName.StartsWith("System.Collection");
            return ret;
        }
    }
}
