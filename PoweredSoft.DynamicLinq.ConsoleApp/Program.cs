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
            var expr = ResolveSelectExpression(param, path, SelectCollectionHandling.Flatten);
            var expr2 = ResolveSelectExpression(param, path, SelectCollectionHandling.Flatten, true);
        }

        public static Expression ResolveSelectExpression(ParameterExpression param, string path, SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs, bool nullChecking = false)
        {
            var notCheckNullExpression = QueryablePathHelpers.NavigatePath(param, path, 
                (before, member, isFirst, isLast) => member, 
                (before, member, isFirst, isLast) => member,
                (parent, innerExpression, innerExpressionLambda) =>
                {
                    var listGenericArgumentType = parent.Type.GetGenericArguments().First();
                    Expression ret = null;
                    if (selectCollectionHandling == SelectCollectionHandling.LeaveAsIs || !QueryableHelpers.IsGenericEnumerable(innerExpression))
                        ret = Expression.Call(typeof(Enumerable), "Select", new Type[] { listGenericArgumentType, innerExpression.Type }, parent, innerExpressionLambda);
                    else
                        ret = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { listGenericArgumentType, innerExpression.Type.GenericTypeArguments.First() }, parent, innerExpressionLambda);

                    return ret;
                }
            );

            if (!nullChecking)
                return notCheckNullExpression;

            var type = notCheckNullExpression.Type;
            throw new NotSupportedException();
        }


    }
}
