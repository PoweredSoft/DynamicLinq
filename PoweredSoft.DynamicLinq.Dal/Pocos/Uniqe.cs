using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Dal.Pocos
{
    public class Unique
    {
        public long Id { get; set; }
        public Guid RowNumber { get; set; }
        public Guid? OtherNullableGuid { get; set; }
    }
}
