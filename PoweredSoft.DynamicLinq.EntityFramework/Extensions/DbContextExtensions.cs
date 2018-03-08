using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PoweredSoft.DynamicLinq.Fluent;

namespace PoweredSoft.DynamicLinq.EntityFramework
{
    public static class DbContextExtensions
    {
        private static readonly MethodInfo QueryMethod = typeof(DbContextExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(t => t.Name == "Query" && t.IsGenericMethod);

        public static IQueryable Query(this DbContext context, Type pocoType, Action<QueryBuilderBase> callback)
        {
            var method = QueryMethod.MakeGenericMethod(pocoType);
            var invokeResult = method.Invoke(null, new object[] {context, callback});
            var ret = invokeResult as IQueryable;
            return ret;
        }

        public static IQueryable<T> Query<T>(this DbContext context, Action<QueryBuilderBase> callback)
            where T : class
        {
            var query = context.Set<T>().AsQueryable();
            query = query.Query(callback);
            return query;
        }

        public static IQueryable Where(this DbContext context, Type pocoType, Action<QueryBuilderBase> callback)
            => context.Query(pocoType, callback);

        public static IQueryable<T> Where<T>(this DbContext context, Action<QueryBuilderBase> callback)
           where T : class => context.Query<T>(callback);
    }
}
