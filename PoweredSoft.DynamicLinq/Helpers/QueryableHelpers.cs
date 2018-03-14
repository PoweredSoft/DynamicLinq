using PoweredSoft.DynamicLinq.DynamicType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


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

        public static IQueryable GroupBy(IQueryable query, Type type, List<(string path, string propertyName)> parts, Type groupToType = null, Type equalityCompareType = null)
        {
            // EXPRESSION
            var parameter = Expression.Parameter(type, "t");
            var partExpressions = new List<(Expression expression, string propertyName)>();

            var fields = new List<(Type type, string propertyName)>();

            // resolve part expression and create the fields inside the anonymous type.
            parts.ForEach(part =>
            {
                var partExpression = ResolvePathForExpression(parameter, part.path);
                fields.Add((partExpression.Type, part.propertyName));
                partExpressions.Add((partExpression, part.propertyName));
            });

            var keyType = groupToType ?? DynamicClassFactory.CreateType(fields);

            /*
            var constructorTypes = fields.Select(t => t.type).ToArray();
            var constructor = anonymousType.GetConstructor(constructorTypes);
            var newExpression = Expression.New(constructor, partExpressions);
            var genericMethod = Constants.GroupByMethod.MakeGenericMethod(type, anonymousType);
            var lambda = Expression.Lambda(newExpression, parameter);
            var groupByExpression = Expression.Call(genericMethod, query.Expression, lambda);
            var result = query.Provider.CreateQuery(groupByExpression);*/

            var ctor = Expression.New(keyType);
            var bindings = partExpressions.Select(partExpression => Expression.Bind(keyType.GetProperty(partExpression.propertyName), partExpression.expression)).ToArray();
            var mi = Expression.MemberInit(ctor, bindings);
            var lambda = Expression.Lambda(mi, parameter);
            var genericMethod = equalityCompareType == null ? Constants.GroupByMethod.MakeGenericMethod(type, keyType) : Constants.GroupByMethodWithEqualityComparer.MakeGenericMethod(type, keyType); //, Activator.CreateInstance(equalityCompareType));
            var groupByExpression = equalityCompareType == null ? Expression.Call(genericMethod, query.Expression, lambda) : Expression.Call(genericMethod, query.Expression, lambda, Expression.New(equalityCompareType));
            var result = query.Provider.CreateQuery(groupByExpression);
            return result;
        }

        public static IQueryable Select(IQueryable query, List<(SelectTypes selectType, string propertyName, string path)> parts, Type destinationType = null)
        {
            // create parameter.
            var queryType = query.ElementType;
            var parameter = Expression.Parameter(queryType, "t");

            // establish which anynomous types we might need to create.
            var fields = new List<(Type type, string propertyName)>();
            var partExpressions = new List<(Expression expression, string propertyName)>();
            parts.ForEach(part =>
            {
                var partBodyExpression = CreateSelectExpression(query, parameter, part.selectType, part.path);
                fields.Add((partBodyExpression.Type, part.propertyName));
                partExpressions.Add((partBodyExpression, part.propertyName));
            });

            // type to use.
            var typeToCreate = destinationType ?? DynamicClassFactory.CreateType(fields);
            var ctor = Expression.New(typeToCreate);
            var bindings = partExpressions.Select(t => Expression.Bind(typeToCreate.GetProperty(t.propertyName), t.expression)).ToArray();
            var mi = Expression.MemberInit(ctor, bindings);
            var lambda = Expression.Lambda(mi, parameter);

            var selectExpr = Expression.Call(typeof(Queryable), "Select", new[] { query.ElementType, typeToCreate }, query.Expression, lambda);
            var result = query.Provider.CreateQuery(selectExpr);
            return result;
        }

        private static Expression CreateSelectExpressionForGrouping(IQueryable query, ParameterExpression parameter, SelectTypes selectType, string path)
        {
            if (selectType == SelectTypes.Key)
            {
                return ResolvePathForExpression(parameter, path);
            }
            else if (selectType == SelectTypes.Count)
            {
                var notGroupedType = parameter.Type.GenericTypeArguments[1];
                var body = Expression.Call(typeof(Enumerable), "Count", new[] { notGroupedType }, parameter);
                return body;
            }
            else if (selectType == SelectTypes.LongCount)
            {
                var notGroupedType = parameter.Type.GenericTypeArguments[1];
                var body = Expression.Call(typeof(Enumerable), "LongCount", new[] { notGroupedType }, parameter);
                return body;
            }
            else if (selectType == SelectTypes.Average)
            {
                /// https://stackoverflow.com/questions/25482097/call-enumerable-average-via-expression
                var notGroupedType = parameter.Type.GenericTypeArguments[1];
                var innerParameter = Expression.Parameter(notGroupedType);
                var innerMemberExpression = ResolvePathForExpression(innerParameter, path);
                var innerMemberLambda = Expression.Lambda(innerMemberExpression, innerParameter);
                var body = Expression.Call(typeof(Enumerable), "Average", new[] { notGroupedType }, parameter, innerMemberLambda);
                return body;
            }
            else if (selectType == SelectTypes.Sum)
            {
                var notGroupedType = parameter.Type.GenericTypeArguments[1];
                var innerParameter = Expression.Parameter(notGroupedType);
                var innerMemberExpression = ResolvePathForExpression(innerParameter, path);
                var innerMemberLambda = Expression.Lambda(innerMemberExpression, innerParameter);
                var body = Expression.Call(typeof(Enumerable), "Sum", new[] { notGroupedType }, parameter, innerMemberLambda);
                return body;
            }
            else if (selectType == SelectTypes.ToList)
            {
                var notGroupedType = parameter.Type.GenericTypeArguments[1];
                var body = Expression.Call(typeof(Enumerable), "ToList", new[] { notGroupedType }, parameter);
                return body;
            }

            throw new NotSupportedException($"unkown select type {selectType}");
        }

        private static Expression CreateSelectExpression(IQueryable query, ParameterExpression parameter, SelectTypes selectType, string path)
        {
            if (!IsGrouping(query))
                throw new NotSupportedException("Select without grouping is not supported yet.");

            return CreateSelectExpressionForGrouping(query, parameter, selectType, path);            
        }

        private static bool IsGrouping(IQueryable query)
        {
            // TODO needs to be alot better than this, but it will do for now.
            if (query.ElementType.Name.Contains("IGrouping"))
                return true;

            return false;
        }

        public static IQueryable GroupBy(IQueryable query, Type type, string path)
        {
            var parameter = Expression.Parameter(type, "t");
            var field = QueryableHelpers.ResolvePathForExpression(parameter, path);
            var lambda = Expression.Lambda(field, parameter);
            var genericMethod = Constants.GroupByMethod.MakeGenericMethod(type, field.Type);
            var groupByEpression = Expression.Call(genericMethod, query.Expression, lambda);
            var result = query.Provider.CreateQuery(groupByEpression);
            return result;
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

        /*
         * var methodInfo = typeof(List<Guid>).GetMethod("Contains", new Type[] { typeof(Guid) });
                var list = Expression.Constant(ids);
                var param = Expression.Parameter(typeof(T), "t");
                var value = Expression.PropertyOrField(param, idField);
                var body = Expression.Call(list, methodInfo, value);
                var lambda = Expression.Lambda<Func<T, bool>>(body, param);
                query = query.Where(lambda);

         */

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
                if (condition == ConditionOperators.In || condition == ConditionOperators.NotIn)
                {
                    return InAndNotIn(parameter, condition, value, convertStrategy, memberExpression);
                }
                else
                {
                    var constant = QueryableHelpers.ResolveConstant(memberExpression, value, convertStrategy);
                    var filterExpression = QueryableHelpers.GetConditionExpressionForMember(parameter, memberExpression, condition, constant, stringComparison);
                    var lambda = Expression.Lambda(filterExpression, parameter);
                    return lambda;
                }
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

        public static Expression InAndNotIn(ParameterExpression parameter, ConditionOperators condition, object value, QueryConvertStrategy convertStrategy, MemberExpression memberExpression)
        {
            var enumerableValue = value as IEnumerable;
            if (enumerableValue == null)
                throw new Exception($"to use {ConditionOperators.In} your value must at least be IEnumerable");

            var enumerableType = GetEnumerableType(enumerableValue);
            var finalType = convertStrategy == QueryConvertStrategy.ConvertConstantToComparedPropertyOrField ? memberExpression.Type : enumerableType;
            var genericListOfEnumerableType = typeof(List<>).MakeGenericType(memberExpression.Type);
            var containsMethod = genericListOfEnumerableType.GetMethod("Contains", new Type[] { finalType });
            var list = Activator.CreateInstance(genericListOfEnumerableType) as IList;
            foreach (var o in enumerableValue)
            {
                if (convertStrategy == QueryConvertStrategy.ConvertConstantToComparedPropertyOrField)
                    list.Add(TypeHelpers.ConvertFrom(memberExpression.Type, o));
                else
                    list.Add(o);
            }

            var body = Expression.Call(Expression.Constant(list), containsMethod, memberExpression) as Expression;

            if (condition == ConditionOperators.NotIn)
                body = Expression.Not(body);

            var lambda = Expression.Lambda(body, parameter);
            return lambda;
        }

        private static Type GetEnumerableType(IEnumerable enumerableValue)
        {
            foreach (var o in enumerableValue)
                return o.GetType();

            return null;
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
