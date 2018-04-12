using PoweredSoft.DynamicLinq.Helpers;
using PoweredSoft.DynamicLinq.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PoweredSoft.DynamicLinq.Resolver
{
    public class PathExpressionResolver
    {
        public bool NullChecking { get; set; } = false;
        public SelectCollectionHandling CollectionHandling { get; set; } = SelectCollectionHandling.LeaveAsIs;
        public ExpressionParser Parser { get; protected set; }

        public Expression Result { get; protected set; }
        public Type NullType { get; set; }

        public PathExpressionResolver(ExpressionParser parser)
        {
            Parser = parser;
        }

        protected Expression CompileGroup(ExpressionParserPieceGroup group, bool nullChecking)
        {
            var expr = group.Parameter as Expression;
            group.Pieces.ForEach(piece =>
            {
                expr = Expression.PropertyOrField(expr, piece.Name);
            });
            return expr;
        }

        public void Resolve()
        {
            Result = null;

            // parse the expression.
            Parser.Parse();

            // group the piece by common parameters
            var groups = Parser.GroupBySharedParameters();

            Expression currentExpression = null;
            foreach (var group in groups.Reversed())
            {
                var isLastGroup = groups.IndexOf(group) == groups.Count - 1;

                if (currentExpression == null)
                {
                    var groupExpression = CompileGroup(group, NullChecking);
                    var groupExpressionLambda = Expression.Lambda(groupExpression, group.Parameter);

                    if (group.Parent == null)
                    {
                        currentExpression = groupExpressionLambda;
                        continue;
                    }

                    var parent = group.Parent;
                    var parentExpression = CompileGroup(parent, NullChecking);

                    // check null with where.
                    if (NullChecking != false)
                        parentExpression = CheckNullOnEnumerableParent(group, parent, parentExpression, isLastGroup);

                    // the select expression.
                    if (CollectionHandling == SelectCollectionHandling.Flatten && QueryableHelpers.IsGenericEnumerable(groupExpression))
                    {
                        var selectType = parent.GroupEnumerableType();
                        var selectExpression = Expression.Call(typeof(Enumerable), "SelectMany",
                            new Type[] { selectType, groupExpression.Type.GenericTypeArguments.First() },
                            parentExpression, groupExpressionLambda);
                        currentExpression = selectExpression;
                    }
                    else
                    {
                        var selectType = parent.GroupEnumerableType();
                        var selectExpression = Expression.Call(typeof(Enumerable), "Select",
                            new Type[] { selectType, groupExpression.Type },
                            parentExpression, groupExpressionLambda);
                        currentExpression = selectExpression;
                    }
                }
                else
                {
                    if (group.Parent == null)
                    {
                        if (NullChecking != false)
                            currentExpression = CheckNullOnFirstGroup(group, currentExpression);

                        currentExpression = Expression.Lambda(currentExpression, group.Parameter);
                        continue;
                    }

                    var parent = group.Parent;
                    var parentExpression = CompileGroup(parent, NullChecking);
                    var selectType = parent.GroupEnumerableType();


                    if (NullChecking != false)
                        parentExpression = CheckNullOnEnumerableParent(group, parent, parentExpression, isLastGroup);

                    if (CollectionHandling == SelectCollectionHandling.Flatten && QueryableHelpers.IsGenericEnumerable(currentExpression))
                    {
                        var currentExpressionLambda = Expression.Lambda(currentExpression, group.Parameter);
                        currentExpression = Expression.Call(typeof(Enumerable), "SelectMany",
                            new Type[] { selectType, currentExpression.Type.GenericTypeArguments.First() },
                            parentExpression, currentExpressionLambda);
                    }
                    else
                    {
                        var currentExpressionLambda = Expression.Lambda(currentExpression, group.Parameter);
                        currentExpression = Expression.Call(typeof(Enumerable), "Select",
                            new Type[] { selectType, currentExpression.Type },
                            parentExpression, currentExpressionLambda);
                    }
                }
            }

            Result = currentExpression;
        }

        private Expression CheckNullOnFirstGroup(ExpressionParserPieceGroup group, Expression currentExpression)
        {
            var path = string.Join(".", group.Pieces.Select(t => t.Name));
            var whereExpression = QueryableHelpers.CreateConditionExpression(group.Parameter.Type, path,
                    ConditionOperators.Equal, null, QueryConvertStrategy.ConvertConstantToComparedPropertyOrField,
                    parameter: group.Parameter, nullChecking: true);

            var whereBodyExpression = (whereExpression as LambdaExpression).Body;

            var nullType = currentExpression.Type;
            Expression ifTrueExpression = null;
            if (QueryableHelpers.IsGenericEnumerable(nullType))
            {
                var listType = typeof(List<>).MakeGenericType(nullType.GenericTypeArguments.First());
                ifTrueExpression = Expression.New(listType);

            }
            else
            {
                ifTrueExpression = Expression.Default(NullType);
            }

            return Expression.Condition(whereBodyExpression, ifTrueExpression, currentExpression, currentExpression.Type);
        }

        private static Expression CheckNullOnEnumerableParent(ExpressionParserPieceGroup group, ExpressionParserPieceGroup parent, Expression parentExpression, bool isLastGroup)
        {
            string path = null;
            if (isLastGroup)
                path = string.Join(".", group.Pieces.Take(group.Pieces.Count - 1).Select(t => t.Name));
            else
                path = string.Join(".", group.Pieces.Select(t => t.Name));

            if (!string.IsNullOrEmpty(path))
            {
                var whereExpression = QueryableHelpers.CreateConditionExpression(group.Parameter.Type, path,
                    ConditionOperators.NotEqual, null, QueryConvertStrategy.ConvertConstantToComparedPropertyOrField,
                    parameter: group.Parameter, nullChecking: true);

                //public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate);
                parentExpression = Expression.Call(typeof(Enumerable), "Where",
                    new Type[] { parent.GroupEnumerableType() },
                    parentExpression, whereExpression);
            }

            return parentExpression;
        }
    }
}
