using System.Collections.Generic;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class OrderByBuilderBase
    {
        public List<OrderByPart> Sorts { get; protected set; } = new List<OrderByPart>();

        public virtual OrderByBuilderBase Sort(string path, QuerySortDirection sortDirection, bool appendSort)
        {
            Sorts.Add(new OrderByPart
            {
                Path = path,
                sortDirection = sortDirection,
                AppendSort = appendSort
            });
            return this;
        }

        public virtual OrderByBuilderBase OrderBy(string path)
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

        public virtual OrderByBuilderBase OrderByDescending(string path)
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

        public virtual OrderByBuilderBase ThenBy(string path)
        {
            Sorts.Add(new OrderByPart
            {
                Path = path,
                sortDirection = QuerySortDirection.Ascending,
                AppendSort = true
            });
            return this;
        }

        public virtual OrderByBuilderBase ThenByDescending(string path)
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
}
