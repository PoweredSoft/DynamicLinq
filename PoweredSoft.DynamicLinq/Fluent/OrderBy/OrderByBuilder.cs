using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Linq;
using System.Text;

namespace PoweredSoft.DynamicLinq.Fluent
{

    public class OrderByBuilder<T> : OrderByBuilderBase
    {
        public IQueryable<T> Query { get; }

        public OrderByBuilder(IQueryable<T> query)
        {
            Query = query;
        }

        public virtual IQueryable<T> Build()
        {
            var query = Query;

            Sorts.ForEach(sort =>
            {
                query = QueryableHelpers.CreateOrderByExpression(query, sort.Path, sort.sortDirection, sort.AppendSort);
            });

            return query;
        }
    }
}
