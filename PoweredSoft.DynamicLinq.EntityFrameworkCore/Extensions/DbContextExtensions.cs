using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PoweredSoft.DynamicLinq.Fluent;

namespace PoweredSoft.DynamicLinq.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
        private static MethodInfo SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), BindingFlags.Public | BindingFlags.Instance);

        public static IQueryable Query(this DbContext context, Type pocoType, Action<WhereBuilder> callback)
        {
            var set = SetMethod.MakeGenericMethod(pocoType).Invoke(context, new object[] { });
            var queryable = set as IQueryable;
            var builder = new WhereBuilder(queryable);
            callback(builder);
            var result = builder.Build();
            return result;
        }

        public static IQueryable<T> Query<T>(this DbContext context, Action<WhereBuilder> callback)
            where T : class
        {
            var query = context.Set<T>().AsQueryable();
            query = query.Query(callback);
            return query;
        }

        public static IQueryable Where(this DbContext context, Type pocoType, Action<WhereBuilder> callback)
            => context.Query(pocoType, callback);

        public static IQueryable<T> Where<T>(this DbContext context, Action<WhereBuilder> callback)
           where T : class => context.Query<T>(callback);
    }
}
