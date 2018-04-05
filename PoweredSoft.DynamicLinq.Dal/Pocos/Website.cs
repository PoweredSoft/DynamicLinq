using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Dal.Pocos
{
    public class Website
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

        public ICollection<Author> Authors { get; set; }
    }
}
