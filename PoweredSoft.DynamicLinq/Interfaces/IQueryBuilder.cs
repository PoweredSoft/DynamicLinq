using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoweredSoft.DynamicLinq
{
    public interface IQueryBuilder
    {
        IQueryable Query { get; }

        IQueryable Build();
    }
}
