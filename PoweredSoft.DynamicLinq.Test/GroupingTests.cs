using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoweredSoft.DynamicLinq;

namespace PoweredSoft.DynamicLinq.Test
{
    [TestClass]
    public class GroupingTests
    {
        [TestMethod]
        public void WantedSyntax()
        {/*
            var regularSyntax = TestData.Sales
                .GroupBy(t => t.ClientId);

            var dynamicSyntax = TestData.Sales
                .AsQueryable()
                .GroupBy("ClientId");

            

            var regularSyntax2 = TestData.Sales
                .GroupBy(t => new
                {
                    t.ClientId,
                    B = t.NetSales
                });*/

            var dynamicSyntax2 = TestData.Sales
                .AsQueryable()
                .GroupBy(t => t.Path("ClientId").Path("NetSales", "B"));

            /*
            .Select(t => new
            {
                TheClientId = t.Key,
                Count = t.Count(),
                CountClientId = t.Count(t2 => t2.ClientId > 1),
                LongCount = t.LongCount(),
                NetSales = t.Sum(t2 => t2.NetSales),
                TaxAverage = t.Average(t2 => t2.Tax),
                Sales = t.ToList()
            });*/



            /*
            .Select(t =>
            {
                t.PropertyFromKey("TheClientId", "ClientId");
                t.Count("Count");
                // don't have to implement right away.
                t.Count("CountClientId", "ClientId", ConditionOperators.GreaterThan, 1);
                t.LongCount("LongCount");
                t.Sum("NetSales");
                t.Average("TaxAverage", "Tax");
                t.ToList("Sales");
            });*/
        }
    }
}
