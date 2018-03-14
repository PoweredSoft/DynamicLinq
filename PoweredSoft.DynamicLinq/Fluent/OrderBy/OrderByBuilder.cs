using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class OrderByBuilder
    {
        public IQueryable Query { get; }

        public OrderByBuilder(IQueryable query)
        {
            Query = query;
        }

        public virtual IQueryable Build()
        {
            var query = Query;

            Sorts.ForEach(sort =>
            {
                query = QueryableHelpers.CreateOrderByExpression(query, sort.Path, sort.Direction, sort.Append);
            });

            return query;
        }

        public List<OrderByPart> Sorts { get; protected set; } = new List<OrderByPart>();

        public virtual OrderByBuilder OrderBy(string path, QueryOrderByDirection direction, bool append)
        {
            if (append == false)
                Sorts.Clear();

            Sorts.Add(new OrderByPart
            {
                Path = path,
                Direction = direction,
                Append = append
            });
            return this;
        }

        #region shortcuts
        public virtual OrderByBuilder OrderBy(string path)
            => OrderBy(path, QueryOrderByDirection.Ascending, false);

        public virtual OrderByBuilder OrderByDescending(string path)
            => OrderBy(path, QueryOrderByDirection.Descending, false);

        public virtual OrderByBuilder ThenBy(string path)
            => OrderBy(path, QueryOrderByDirection.Ascending, true);

        public virtual OrderByBuilder ThenByDescending(string path)
            => OrderBy(path, QueryOrderByDirection.Descending, true);
        #endregion
    }
}
