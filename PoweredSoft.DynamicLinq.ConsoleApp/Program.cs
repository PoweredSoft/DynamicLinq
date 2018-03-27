using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Helpers;
using PoweredSoft.DynamicLinq.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var type = typeof(Dal.Pocos.Author);
            var param = Expression.Parameter(type);
            var parts = ExpressionPathPart.Split(param, "Posts.Comments.Id");

            // add the last part.
            var parent = parts.Last();
            var toListType = parent.GetToListType();
            parts.Add(new ExpressionPathPart
            {
                ParentExpression = parent.PartExpression,
                PartExpression = Expression.Call(typeof(Enumerable), "ToList", new[] { toListType }, parent.ParentExpression)
            });

            var result = CreateSelectExpressionFromParts(parts, SelectCollectionHandling.Flatten, SelectNullHandling.New);
        }

        public static Expression CreateSelectExpressionFromParts(List<ExpressionPathPart> parts, 
            SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs, 
            SelectNullHandling nullChecking = SelectNullHandling.LeaveAsIs)
        {
            Type nullHandlingRightType = null;
            Expression nullHandlingNullValue = null;

            if (nullChecking != SelectNullHandling.LeaveAsIs)
            {
                nullHandlingRightType = parts.Last().PartExpression.Type;
                nullHandlingNullValue = nullChecking == SelectNullHandling.Default
                    ? Expression.Default(nullHandlingRightType) as Expression
                    : Expression.New(nullHandlingRightType) as Expression;
            }

            // reversed :)
            var reversedCopy = parts.Select(t => t).ToList();
            reversedCopy.Reverse();

            Expression ret = null;
            reversedCopy.ForEach(t =>
            {
                if (t.IsParentGenericEnumerable())
                {
                    var lambda = t.GetLambdaExpression();
                    var parentGenericType = t.ParentGenericEnumerableType();
                    if (selectCollectionHandling == SelectCollectionHandling.LeaveAsIs || !t.IsGenericEnumerable())
                        ret = Expression.Call(typeof(Enumerable), "Select", new Type[] { parentGenericType, t.PartExpression.Type }, t.ParentExpression, lambda);
                    else
                        ret = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { parentGenericType, t.GenericEnumerableType() }, t.ParentExpression, lambda);
                }
                else
                {
                    ret = t.PartExpression;
                }
            });
            
            return ret;
        }



        /*
         *  (parent, innerExpression, innerExpressionLambda) =>
                {
                    var listGenericArgumentType = parent.Type.GetGenericArguments().First();
                    Expression ret = null;
                    if (selectCollectionHandling == SelectCollectionHandling.LeaveAsIs || !QueryableHelpers.IsGenericEnumerable(innerExpression))
                        ret = Expression.Call(typeof(Enumerable), "Select", new Type[] { listGenericArgumentType, innerExpression.Type }, parent, innerExpressionLambda);
                    else
                        ret = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { listGenericArgumentType, innerExpression.Type.GenericTypeArguments.First() }, parent, innerExpressionLambda);

                    return ret;
                }
         */
    }
}
