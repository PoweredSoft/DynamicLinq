using PoweredSoft.DynamicLinq.DynamicType;
using PoweredSoft.DynamicLinq.Parser;
using PoweredSoft.DynamicLinq.Resolver;
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
            else if (conditionOperator == ConditionOperators.NotContains)
            {
                if (member.Type == stringType && stringComparision.HasValue)
                    ret = Expression.GreaterThan(Expression.Not(Expression.Call(member, Constants.IndexOfMethod, constant, Expression.Constant(stringComparision.Value))), Expression.Constant(-1));
                else
                    ret = Expression.Not(Expression.Call(member, Constants.ContainsMethod, constant));
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

  

        public static IQueryable GroupBy(IQueryable query, Type type, List<(string path, string propertyName)> parts, Type groupToType = null, Type equalityCompareType = null, bool nullChecking = false)
        {
            // EXPRESSION
            var parameter = Expression.Parameter(type, "t");
            var partExpressions = new List<(Expression expression, string propertyName)>();

            var fields = new List<(Type type, string propertyName)>();

            // resolve part expression and create the fields inside the anonymous type.
            parts.ForEach(part =>
            {
                var partExpression = CreateSelectExpression(query, parameter, SelectTypes.Path, part.path, SelectCollectionHandling.LeaveAsIs, nullChecking: nullChecking);
                fields.Add((partExpression.Type, part.propertyName));
                partExpressions.Add((partExpression, part.propertyName));
            });

            var keyType = groupToType ?? DynamicClassFactory.CreateType(fields);
            var ctor = Expression.New(keyType);
            var bindings = partExpressions.Select(partExpression => Expression.Bind(keyType.GetProperty(partExpression.propertyName), partExpression.expression)).ToArray();
            var mi = Expression.MemberInit(ctor, bindings);
            var lambda = Expression.Lambda(mi, parameter);
            var genericMethod = equalityCompareType == null ? Constants.GroupByMethod.MakeGenericMethod(type, keyType) : Constants.GroupByMethodWithEqualityComparer.MakeGenericMethod(type, keyType); //, Activator.CreateInstance(equalityCompareType));
            var groupByExpression = equalityCompareType == null ? Expression.Call(genericMethod, query.Expression, lambda) : Expression.Call(genericMethod, query.Expression, lambda, Expression.New(equalityCompareType));
            var result = query.Provider.CreateQuery(groupByExpression);
            return result;
        }

        public static IQueryable Select(IQueryable query, List<(SelectTypes selectType, string propertyName, string path, SelectCollectionHandling selectCollectionHandling)> parts, Type destinationType = null, bool nullChecking = false)
        {
            // create parameter.
            var queryType = query.ElementType;
            var parameter = Expression.Parameter(queryType, "t");

            // establish which anynomous types we might need to create.
            var fields = new List<(Type type, string propertyName)>();
            var partExpressions = new List<(Expression expression, string propertyName)>();
            parts.ForEach(part =>
            {
                var partBodyExpression = CreateSelectExpression(query, parameter, part.selectType, part.path, part.selectCollectionHandling, nullChecking);
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

        private static Expression CreateSelectExpressionForGrouping(IQueryable query, ParameterExpression parameter, SelectTypes selectType, string path, SelectCollectionHandling selectCollectionHandling, bool nullChecking)
        {
            if (selectType == SelectTypes.Key)
            {
                var parser = new ExpressionParser(parameter, path);
                var resolver = new PathExpressionResolver(parser);
                resolver.NullChecking = nullChecking;
                resolver.Resolve();
                return resolver.GetResultBodyExpression();
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
                return CreateGroupedAggregateExpression(parameter, path, "Average");
            else if (selectType == SelectTypes.Sum)
                return CreateGroupedAggregateExpression(parameter, path, "Sum");
            else if (selectType == SelectTypes.Min)
                return CreateGroupedAggregateExpression(parameter, path, "Min");
            else if (selectType == SelectTypes.Max)
                return CreateGroupedAggregateExpression(parameter, path, "Max");
            else if (selectType == SelectTypes.ToList)
                return CreateGroupedPathExpressionWithMethod(parameter, path, selectCollectionHandling, nullChecking, "ToList");
            else if (selectType == SelectTypes.First)
                return CreateGroupedPathExpressionWithMethod(parameter, path, selectCollectionHandling, nullChecking, "First");
            else if (selectType == SelectTypes.Last)
                return CreateGroupedPathExpressionWithMethod(parameter, path, selectCollectionHandling, nullChecking, "Last");
            else if (selectType == SelectTypes.FirstOrDefault)
                return CreateGroupedPathExpressionWithMethod(parameter, path, selectCollectionHandling, nullChecking, "FirstOrDefault");
            else if (selectType == SelectTypes.LastOrDefault)
                return CreateGroupedPathExpressionWithMethod(parameter, path, selectCollectionHandling, nullChecking, "LastOrDefault");

            throw new NotSupportedException($"unkown select type {selectType}");
        }

        private static Expression CreateGroupedAggregateExpression(ParameterExpression parameter, string path, string methodName)
        {
            /// https://stackoverflow.com/questions/25482097/call-enumerable-average-via-expression
            var notGroupedType = parameter.Type.GenericTypeArguments[1];
            var innerParameter = Expression.Parameter(notGroupedType);
            var innerMemberExpression = ResolvePathForExpression(innerParameter, path);
            var innerMemberLambda = Expression.Lambda(innerMemberExpression, innerParameter);
            var body = Expression.Call(typeof(Enumerable), methodName, new[] { notGroupedType }, parameter, innerMemberLambda);
            return body;
        }

        private static Expression CreateGroupedPathExpressionWithMethod(ParameterExpression parameter, string path, SelectCollectionHandling selectCollectionHandling, bool nullChecking, string methodName)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                var notGroupedType = parameter.Type.GenericTypeArguments[1];
                var body = Expression.Call(typeof(Enumerable), methodName, new[] { notGroupedType }, parameter);
                return body;
            }
            else
            {
                var notGroupedType = parameter.Type.GenericTypeArguments[1];
                var innerParameter = Expression.Parameter(notGroupedType);
                var parser = new ExpressionParser(innerParameter, path);
                var resolver = new PathExpressionResolver(parser);
                resolver.NullChecking = nullChecking;
                resolver.Resolve();
                var expression = resolver.Result;
                var selectExpression = WrapIntoSelectFromGrouping(parameter, expression, selectCollectionHandling);
                var body = CallMethodOnSelectExpression(methodName, selectExpression);
                return body;
            }
        }

        private static Expression WrapIntoSelectFromGrouping(ParameterExpression parameter, Expression innerLambdaExpression, SelectCollectionHandling selectCollectionHandling)
        {
            var selectType = parameter.Type.GenericTypeArguments.Skip(1).First();
            var innerSelectType = ((LambdaExpression)innerLambdaExpression).ReturnType;

            Expression selectExpression;
            if (QueryableHelpers.IsGenericEnumerable(innerSelectType) && selectCollectionHandling == SelectCollectionHandling.Flatten)
                selectExpression = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { selectType, innerSelectType.GenericTypeArguments.First() }, parameter, innerLambdaExpression);
            else
                selectExpression = Expression.Call(typeof(Enumerable), "Select", new Type[] { selectType, innerSelectType }, parameter, innerLambdaExpression);

            return selectExpression;
        }
    
        private static Expression CreateSelectExpression(IQueryable query, ParameterExpression parameter, SelectTypes selectType, string path, SelectCollectionHandling selectCollectionHandling, bool nullChecking)
        {
            if (!IsGrouping(query))
                return CreateSelectExpressionRegular(query, parameter, selectType, path, selectCollectionHandling, nullChecking);

            return CreateSelectExpressionForGrouping(query, parameter, selectType, path, selectCollectionHandling, nullChecking);            
        }

        private static Expression CreateSelectExpressionRegular(IQueryable query, ParameterExpression parameter, SelectTypes selectType, string path
            , SelectCollectionHandling selectCollectionHandling, bool nullChecking)
        {
            if (selectType == SelectTypes.Path)
            {
                var parser = new ExpressionParser(parameter, path);
                var resolver = new PathExpressionResolver(parser);
                resolver.NullChecking = nullChecking;
                resolver.Resolve();
                return resolver.GetResultBodyExpression();
            }
            else if (selectType == SelectTypes.ToList)
                return CreateSelectExpressionPathWithMethodName(parameter, path, selectCollectionHandling, nullChecking, "ToList");
            else if (selectType == SelectTypes.First)
                return CreateSelectExpressionPathWithMethodName(parameter, path, selectCollectionHandling, nullChecking, "First");
            else if (selectType == SelectTypes.FirstOrDefault)
                return CreateSelectExpressionPathWithMethodName(parameter, path, selectCollectionHandling, nullChecking, "FirstOrDefault");
            else if (selectType == SelectTypes.Last)
                return CreateSelectExpressionPathWithMethodName(parameter, path, selectCollectionHandling, nullChecking, "Last");
            else if (selectType == SelectTypes.LastOrDefault)
                return CreateSelectExpressionPathWithMethodName(parameter, path, selectCollectionHandling, nullChecking, "LastOrDefault");


            throw new NotSupportedException($"unkown select type {selectType}");
        }

        private static Expression CreateSelectExpressionPathWithMethodName(ParameterExpression parameter, string path, SelectCollectionHandling selectCollectionHandling, bool nullChecking, string methodName)
        {
            var parser = new ExpressionParser(parameter, path);
            var resolver = new PathExpressionResolver(parser);
            resolver.NullChecking = nullChecking;
            resolver.CollectionHandling = selectCollectionHandling;
            resolver.Resolve();
            var expr = (resolver.Result as LambdaExpression).Body;
            var notGroupedType = expr.Type.GenericTypeArguments.FirstOrDefault();
            if (notGroupedType == null)
                throw new Exception($"Path must be a Enumerable<T> but its a {expr.Type}");

            var body = Expression.Call(typeof(Enumerable), methodName, new[] { notGroupedType }, expr) as Expression;
            return body;
        }

        private static Expression CallMethodOnSelectExpression(string methodName, Expression selectExpression)
        {
            var notGroupedType = selectExpression.Type.GenericTypeArguments.First();
            var body = Expression.Call(typeof(Enumerable), methodName, new[] { notGroupedType }, selectExpression) as Expression;
            return body;
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

        internal static Expression InternalResolvePathExpression(int step, Expression param, List<string> parts, SelectCollectionHandling selectCollectionHandling, bool nullChecking, Type nullCheckingType = null)
        {
            var isLast = parts.Count == 1;
            var currentPart = parts.First();
            var memberExpression = Expression.PropertyOrField(param, currentPart);


            if (isLast)
                return memberExpression;

            Expression ret = null;

            if (!IsGenericEnumerable(memberExpression))
            {
                // TODO: null checking here too.
                // should be easier then collection :=|

                ret = InternalResolvePathExpression(step + 1, memberExpression, parts.Skip(1).ToList(), selectCollectionHandling, nullChecking, nullCheckingType: nullCheckingType);
            }
            else
            {
                // enumerable.
                var listGenericArgumentType = memberExpression.Type.GetGenericArguments().First();

                // sub param.
                var innerParam = Expression.Parameter(listGenericArgumentType);
                var innerExpression = InternalResolvePathExpression(step + 1, innerParam, parts.Skip(1).ToList(), selectCollectionHandling, nullChecking);
                var lambda = Expression.Lambda(innerExpression, innerParam);

                if (selectCollectionHandling == SelectCollectionHandling.LeaveAsIs)
                    ret = Expression.Call(typeof(Enumerable), "Select", new Type[] { listGenericArgumentType, innerExpression.Type }, memberExpression, lambda);
                else
                    ret = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { listGenericArgumentType, innerExpression.Type.GenericTypeArguments.First() }, memberExpression, lambda);
            }

            if (step == 1 && nullChecking)
            {
                ret = Expression.Condition(
                    Expression.Equal(memberExpression, Expression.Constant(null)),
                    Expression.New(nullCheckingType),
                    ret
                    );
            }

            return ret;            
        }

        /// <summary>
        /// Returns the right expression for a path supplied.
        /// </summary>
        /// <param name="param">Expression.Parameter(typeOfClassOrInterface)</param>
        /// <param name="path">the path you wish to resolve example Contact.Profile.FirstName</param>
        /// <returns></returns>
        public static Expression ResolvePathForExpression(ParameterExpression param, string path, bool throwIfHasEnumerable = true)
        {
            var expressionParser = new ExpressionParser(param, path);
            expressionParser.Parse();

            if (throwIfHasEnumerable && expressionParser.Pieces.Any(t2 => t2.IsGenericEnumerable))
                throw new Exception("Path contains an enumerable, and this feature does not support it.");

            var expressionResolver = new PathExpressionResolver(expressionParser);
            expressionResolver.Resolve();
            return expressionResolver.GetResultBodyExpression();
        }

        public static ConstantExpression GetConstantSameAsLeftOperator(Expression member, object value)
        {
            if (member == null)
                throw new ArgumentNullException("member");

            if (value == null)
                return Expression.Constant(null);

            var convertedValue = PoweredSoft.Types.Converter.To(value, member.Type);
            return Expression.Constant(convertedValue, member.Type);
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

        public static IQueryable CreateOrderByExpression(IQueryable query, string path, QueryOrderByDirection direction, bool append = true)
        {
            var parameter = Expression.Parameter(query.ElementType, "t");
            var member = QueryableHelpers.ResolvePathForExpression(parameter, path);

            string sortCommand = null;
            if (direction == QueryOrderByDirection.Descending)
                sortCommand = append == false ? "OrderByDescending" : "ThenByDescending";
            else
                sortCommand = append == false ? "OrderBy" : "ThenBy";

            var expression = Expression.Lambda(member, parameter);

            var resultExpression = Expression.Call
                    (typeof(Queryable),
                    sortCommand,
                    new Type[] { query.ElementType, member.Type },
                    query.Expression,
                    Expression.Quote(expression)
                );

            query = query.Provider.CreateQuery(resultExpression);
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

        internal static Expression InternalCreateConditionExpression(int recursionStep, Type type, ParameterExpression parameter, Expression current, List<string> parts,
            ConditionOperators condition, object value, QueryConvertStrategy convertStrategy, QueryCollectionHandling collectionHandling, bool nullChecking, StringComparison? stringComparison)
        {
            var partStr = parts.First();
            var isLast = parts.Count == 1;

            // the member expression.
            var memberExpression = Expression.PropertyOrField(current, partStr);

            // TODO : maybe support that last part is collection but what do we do?
            // not supported yet.
            if (isLast && IsGenericEnumerable(memberExpression) && value != null)
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

            if (IsGenericEnumerable(memberExpression))
            {
                var listGenericArgumentType = memberExpression.Type.GetGenericArguments().First();
                var innerParameter = Expression.Parameter(listGenericArgumentType, $"t{++recursionStep}");
                var innerLambda = InternalCreateConditionExpression(recursionStep, listGenericArgumentType, innerParameter, innerParameter, parts.Skip(1).ToList(), condition, value, convertStrategy, collectionHandling, nullChecking, stringComparison);

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
                    var pathExpr = InternalCreateConditionExpression(recursionStep, type, parameter, memberExpression, parts.Skip(1).ToList(), condition, value, convertStrategy, collectionHandling, nullChecking, stringComparison);
                    
                    var nullCheckResult = Expression.AndAlso(nullCheckExpression, (pathExpr as LambdaExpression).Body);
                    var nullCheckResultLambda = Expression.Lambda((Expression)nullCheckResult, parameter);
                    return nullCheckResultLambda;
                }

                return InternalCreateConditionExpression(recursionStep, type, parameter, memberExpression, parts.Skip(1).ToList(), condition, value, convertStrategy, collectionHandling, nullChecking, stringComparison);
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
                    list.Add(PoweredSoft.Types.Converter.To(o, memberExpression.Type));
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


        public static Expression<Func<T, bool>> CreateConditionExpression<T>(
            string path,
            ConditionOperators condition,
            object value,
            QueryConvertStrategy convertStrategy,
            QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any,
            ParameterExpression parameter = null,
            bool nullChecking = false,
            StringComparison? stringComparision = null)
        {
            var ret = CreateConditionExpression(typeof(T), path, condition, value, convertStrategy,
                collectionHandling: collectionHandling,
                parameter: parameter,
                nullChecking: nullChecking,
                stringComparision: stringComparision) as Expression<Func<T, bool>>;
            return ret;
        }

        public static Expression CreateConditionExpression(Type type, 
            string path, 
            ConditionOperators condition, 
            object value, 
            QueryConvertStrategy convertStrategy, 
            QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any,
            ParameterExpression parameter = null,
            bool nullChecking = false,
            StringComparison? stringComparision = null)
        {
            if (parameter == null)
                parameter = Expression.Parameter(type, "t");

            var parts = path.Split('.').ToList();
            var result = InternalCreateConditionExpression(1, type, parameter, parameter, parts, condition, value, convertStrategy, collectionHandling, nullChecking, stringComparision);
            return result;
        }

        public static bool IsGenericEnumerable(Expression member) => IsGenericEnumerable(member.Type);
        public static bool IsGenericEnumerable(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var genericArgumentType = type.GenericTypeArguments.First();
            var ret = typeof(IEnumerable<>).MakeGenericType(genericArgumentType).IsAssignableFrom(type);
            return ret;
        }
    }
}
