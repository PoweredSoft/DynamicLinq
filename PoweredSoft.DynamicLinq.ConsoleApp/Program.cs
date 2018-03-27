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
            var path = "Posts.Comments.Id";

            var parts = ExpressionPathPart.Break(param, "Posts.Comments.Id");

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
