using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PoweredSoft.DynamicLinq.Helpers
{
    public class ExpressionPathPart
    {
        public Expression ParentExpression { get; set; }
        public Expression Expression { get; set; }
        public bool IsNullable() => TypeHelpers.IsNullable(Expression.Type);
        public bool IsParentParamaterExpression() => ParentExpression is ParameterExpression;
        public bool IsGenericEnumerable() => QueryableHelpers.IsGenericEnumerable(Expression);
        public Type GenericEnumerableType() => Expression.Type.GenericTypeArguments.First();

        public static List<ExpressionPathPart> Break(ParameterExpression param, string path)
        {
            var ret = new List<ExpressionPathPart>();

            var parts = path.Split('.').ToList();
            Expression parent = param;
            parts.ForEach(part =>
            {
                var p = new ExpressionPathPart();
                p.Expression = Expression.PropertyOrField(parent, part);
                p.ParentExpression = parent;
                ret.Add(p);

                parent = p.IsGenericEnumerable() ? Expression.Parameter(p.GenericEnumerableType()) : p.Expression;
            });

            return ret;
        }
    }
}
