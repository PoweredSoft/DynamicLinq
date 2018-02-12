using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class QueryBuilderSort
    {
        public string Path { get; set; }

        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

        public bool AppendSort { get; set; }
    }
}
