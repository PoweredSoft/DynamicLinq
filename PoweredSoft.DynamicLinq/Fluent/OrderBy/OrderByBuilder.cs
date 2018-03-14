using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class SortBuilderBase
    {
        public List<OrderByPart> Sorts { get; protected set; } = new List<OrderByPart>();

        public virtual SortBuilderBase Sort(string path, QuerySortDirection sortDirection, bool appendSort)
        {
            Sorts.Add(new OrderByPart
            {
                Path = path,
                sortDirection = sortDirection,
                AppendSort = appendSort
            });
            return this;
        }

        public virtual SortBuilderBase OrderBy(string path)
        {
            Sorts.Clear();
            Sorts.Add(new OrderByPart
            {
                Path = path,
                sortDirection = QuerySortDirection.Ascending,
                AppendSort = false
            });
            return this;
        }

        public virtual SortBuilderBase OrderByDescending(string path)
        {
            Sorts.Clear();
            Sorts.Add(new OrderByPart
            {
                Path = path,
                sortDirection = QuerySortDirection.Descending,
                AppendSort = false
            });
            return this;
        }

        public virtual SortBuilderBase ThenBy(string path)
        {
            Sorts.Add(new OrderByPart
            {
                Path = path,
                sortDirection = QuerySortDirection.Ascending,
                AppendSort = true
            });
            return this;
        }

        public virtual SortBuilderBase ThenByDescending(string path)
        {
            Sorts.Add(new OrderByPart
            {
                Path = path,
                sortDirection = QuerySortDirection.Descending,
                AppendSort = true
            });
            return this;
        }
    }

    public class OrderByBuilder<T> : SortBuilderBase
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
                query = QueryableHelpers.CreateSortExpression(query, sort.Path, sort.sortDirection, sort.AppendSort);
            });

            return query;
        }
    }
}
