using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class QueryFilterPart
    {
        public string Path { get; set; }
        public ConditionOperators ConditionOperator { get; set; }
        public object Value { get; set; }
        public bool And { get; set; }
        public QueryConvertStrategy ConvertStrategy { get; set; }
        public List<QueryFilterPart> Parts { get; set; } = new List<QueryFilterPart>();
    }
}
