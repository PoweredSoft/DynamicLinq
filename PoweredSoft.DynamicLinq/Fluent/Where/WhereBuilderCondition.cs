using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class WhereBuilderCondition
    {
        public string Path { get; set; }
        public ConditionOperators ConditionOperator { get; set; }
        public object Value { get; set; }
        public bool And { get; set; }
        public QueryConvertStrategy ConvertStrategy { get; set; }
        public List<WhereBuilderCondition> Conditions { get; set; } = new List<WhereBuilderCondition>();
        public QueryCollectionHandling CollectionHandling { get;  set; }
        public StringComparison? StringComparisation { get; set; } = null;
        public bool Negate { get; set; } = false;
    }
}
