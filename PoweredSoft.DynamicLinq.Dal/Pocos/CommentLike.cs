using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Dal.Pocos
{
    public class CommentLike
    {
        public long Id { get; set; }
        public long CommentId { get; set; }
        public DateTimeOffset CreateTime { get; set; }

        public virtual Comment Comment { get; set; }
    }
}
