using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Dal.Pocos
{
    public class Author
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? WebsiteId { get; set; }

        public virtual Website Website { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
