using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Helpers;
using PoweredSoft.DynamicLinq.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.ConsoleApp
{
    public class SelectExpression
    {
        static void Main(string[] args)
        {
            var selectTests = new SelectTests();
            selectTests.SelectNullChecking();
        }

    }
}
