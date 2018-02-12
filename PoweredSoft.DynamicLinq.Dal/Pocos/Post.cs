using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Dal.Pocos
{
    public class Post
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        public DateTimeOffset? PublishTime { get; set; }

        public Author Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
