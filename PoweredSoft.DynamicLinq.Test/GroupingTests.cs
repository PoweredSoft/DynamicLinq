using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoweredSoft.DynamicLinq;
using PoweredSoft.DynamicLinq.Dal;
using System.Diagnostics;

namespace PoweredSoft.DynamicLinq.Test
{
    internal class TestStructureCompare : IEqualityComparer<TestStructure> 
    {
        public bool Equals(TestStructure x, TestStructure y)
        {
            return x?.ClientId == y?.ClientId;
        }

        public int GetHashCode(TestStructure obj)
        {
            return obj.ClientId;
        }
    }

    internal class TestStructure 
    {
        public int ClientId { get; set; }
    }

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

            /*
            var dynamicSyntax2 = TestData.Sales
                .AsQueryable()
                .GroupBy(t => t.Path("ClientId").Path("NetSales", "B"));

            var dynamicSyntax3 = TestData.Sales
                .AsQueryable()
                .GroupBy(t => t.UseType(typeof(TestStructure)).EqualityComparer(typeof(TestStructureCompare)).Path("ClientId"));

            var tryAs = dynamicSyntax3 as EnumerableQuery<IGrouping<TestStructure, MockSale>>;
            var list = tryAs.Select(t => new
            {
                Key = t.Key,
                Data = t.ToList()
            }).ToList();

            var list2 = TestData.Sales.GroupBy(t => new TestStructure { ClientId = t.ClientId }, new TestStructureCompare()).Select(t => new
            {
                Key = t.Key,
                Data = t.ToList()
            }).ToList();

            int i = 0;*/

            /*
            var dynamicSyntax3 = TestData.Sales
               .AsQueryable()
               .GroupBy(t => t.ClientId)
                .Select(t => new
                {
                    TheClientId = t.Key,
                    Count = t.Count(),
                });*/

            /*
            TestData.Sales.GroupBy(t => new { t.ClientId }).Select(t => new
            {
                TheClientId = t.Key.ClientId,
                Count = t.Count(),
                LongCount = t.LongCount(),
                TaxAverage = t.Average(t2 => t2.Tax)
            });*/

            var dynamicSyntax2 = TestData.Sales
               .AsQueryable()
               .GroupBy(t => t.Path("ClientId"))
               .Select(t =>
               {
                    t.Key("TheClientId", "ClientId");
                    t.Count("Count");
                    t.LongCount("LongCount");
                    t.Sum("NetSales");
                    t.Average("Tax", "TaxAverage");
                    t.ToList("Sales");
               });


            var result = dynamicSyntax2.ToObjectList();
        }
    }
}
