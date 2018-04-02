using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PoweredSoft.DynamicLinq.Helpers
{
    public class ExpressionPathPart
    {
        public Expression ParentExpression { get; set; }
        public ParameterExpression ParameterExpression { get; set; }
        public Expression PartExpression { get; set; }

        public bool IsNullable() => TypeHelpers.IsNullable(PartExpression.Type);
   
        public bool IsGenericEnumerable() => QueryableHelpers.IsGenericEnumerable(PartExpression);
        public Type GenericEnumerableType() => PartExpression.Type.GenericTypeArguments.First();
        public bool IsCallingMethod() => PartExpression is MethodCallExpression;
        public Type GetToListType() => IsGenericEnumerable() ? GenericEnumerableType() : PartExpression.Type;
        public bool IsParentGenericEnumerable() => QueryableHelpers.IsGenericEnumerable(ParentExpression);

        public static List<ExpressionPathPart> Split(ParameterExpression param, string path)
        {
            var ret = new List<ExpressionPathPart>();

            var parts = path.Split('.').ToList();
            Expression parent = param;
            parts.ForEach(part =>
            {
                var p = new ExpressionPathPart();
                p.PartExpression = Expression.PropertyOrField(parent, part);
                p.ParentExpression = parent;
                p.ParameterExpression = param;
                ret.Add(p);

                if (p.IsGenericEnumerable())
                {
                    param = Expression.Parameter(p.GenericEnumerableType());
                    parent = param;
                }
                else
                {
                    parent = p.PartExpression;
                }
            });

            return ret;
        }

        public LambdaExpression GetLambdaExpression()
        {
            var lambda = Expression.Lambda(PartExpression, ParameterExpression);
            return lambda;
        }

        public Type ParentGenericEnumerableType() => ParentExpression.Type.GenericTypeArguments.First();

    }
}
