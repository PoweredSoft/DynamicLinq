using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class QueryBuilderSort
    {
        public string Path { get; set; }

        public QuerySortDirection sortDirection { get; set; } = QuerySortDirection.Ascending;

        public bool AppendSort { get; set; }
    }
}
