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
        public static IQueryable Query(this DbContext context, Type pocoType, Action<WhereBuilder> callback)
        {
            var set = context.Set(pocoType);
            var queryable = set.AsQueryable();
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
