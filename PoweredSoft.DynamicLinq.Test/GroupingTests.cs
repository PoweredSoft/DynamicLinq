using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoweredSoft.DynamicLinq;
using PoweredSoft.DynamicLinq.Dal;
using System.Diagnostics;
using PoweredSoft.DynamicLinq.Test.Helpers;
using System.Collections;

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
        public void TestEmptyGroup()
        {
            var subject = TestData.Sales;

            var normalSyntax = subject
                .GroupBy(t => true)
                .Select(t => new
                {
                    NetSalesSum = t.Sum(t2 => t2.NetSales),
                    NetSalesAvg = t.Average(t2 => t2.NetSales)
                })
                .First();

            var dynamicSyntax = subject
                .EmptyGroupBy(typeof(MockSale))
                .Select(sb =>
                {
                    sb.Sum("NetSales", "NetSalesSum");
                    sb.Average("NetSales", "NetSalesAvg");
                })
                .ToDynamicClassList()
                .First();

            Assert.AreEqual(normalSyntax.NetSalesAvg, dynamicSyntax.GetDynamicPropertyValue<decimal>("NetSalesAvg"));
            Assert.AreEqual(normalSyntax.NetSalesSum, dynamicSyntax.GetDynamicPropertyValue<decimal>("NetSalesSum"));
        }

        [TestMethod]
        public void WantedSyntax()
        {
            var normalSyntax = TestData.Sales
                .GroupBy(t => new { t.ClientId })
                .Select(t => new
                {
                    TheClientId = t.Key.ClientId,
                    Count = t.Count(),
                    LongCount = t.LongCount(),
                    NetSales = t.Sum(t2 => t2.NetSales),
                    TaxAverage = t.Average(t2 => t2.Tax),
                    Sales = t.ToList(),
                    MaxNetSales = t.Max(t2 => t2.NetSales),
                    MinNetSales = t.Min(t2 => t2.NetSales),
                    First = t.First(),
                    Last = t.Last(),
                    FirstOrDefault = t.FirstOrDefault(),
                    LastOrDefault = t.LastOrDefault()
                })
                .ToList();

            var dynamicSyntax = TestData.Sales
               .AsQueryable()
               .GroupBy(t => t.Path("ClientId"))
               .Select(t =>
               {
                   t.Key("TheClientId", "ClientId");
                   t.Count("Count");
                   t.LongCount("LongCount");
                   t.Sum("NetSales");
                   t.Average("Tax", "TaxAverage");
                   t.Max("NetSales", "MaxNetSales");
                   t.Min("NetSales", "MinNetSales");
                   t.First("First");
                   t.Last("Last");
                   t.FirstOrDefault("FirstOrDefault");
                   t.LastOrDefault("LastOrDefault");
                   t.ToList("Sales");
               })
               .ToDynamicClassList();

            Assert.AreEqual(normalSyntax.Count, dynamicSyntax.Count);
            for(var i = 0; i < normalSyntax.Count; i++)
            {
                var left = normalSyntax[i];
                var right = dynamicSyntax[i];

                Assert.AreEqual(left.TheClientId, right.GetDynamicPropertyValue("TheClientId"));
                Assert.AreEqual(left.Count, right.GetDynamicPropertyValue("Count"));
                Assert.AreEqual(left.LongCount, right.GetDynamicPropertyValue("LongCount"));
                Assert.AreEqual(left.TaxAverage, right.GetDynamicPropertyValue("TaxAverage"));
                Assert.AreEqual(left.MinNetSales, right.GetDynamicPropertyValue("MinNetSales"));
                Assert.AreEqual(left.MaxNetSales, right.GetDynamicPropertyValue("MaxNetSales"));

                Assert.AreEqual(left.First, right.GetDynamicPropertyValue("First"));
                Assert.AreEqual(left.FirstOrDefault, right.GetDynamicPropertyValue("FirstOrDefault"));
                Assert.AreEqual(left.Last, right.GetDynamicPropertyValue("Last"));
                Assert.AreEqual(left.LastOrDefault, right.GetDynamicPropertyValue("LastOrDefault"));

                QueryableAssert.AreEqual(left.Sales.AsQueryable(), right.GetDynamicPropertyValue<List<MockSale>>("Sales").AsQueryable());
            }
        }

        [TestMethod]
        public void TestingSelectBuilderAggregateFluent()
        {
            var normalSyntax = TestData.Sales
                .GroupBy(t => new { t.ClientId })
                .Select(t => new
                {
                    TheClientId = t.Key.ClientId,
                    Count = t.Count(),
                    LongCount = t.LongCount(),
                    NetSales = t.Sum(t2 => t2.NetSales),
                    TaxAverage = t.Average(t2 => t2.Tax),
                    Sales = t.ToList()
                })
                .ToList();

            var dynamicSyntax = TestData.Sales
               .AsQueryable()
               .GroupBy(t => t.Path("ClientId"))
               .Select(t =>
               {
                   t.Aggregate("Key.ClientId", SelectTypes.Key, "TheClientId");
                   // should not have to specify a path, but a property is a must
                   t.Aggregate(null, SelectTypes.Count, "Count"); 
                   // support both ways it can use path to guess property so testing this too
                   t.Aggregate("LongCount", SelectTypes.LongCount); 
                   t.Aggregate("NetSales", SelectTypes.Sum);
                   t.Aggregate("Tax", SelectTypes.Average, "TaxAverage");
                   t.ToList("Sales");
               })
               .ToDynamicClassList();

            Assert.AreEqual(normalSyntax.Count, dynamicSyntax.Count);
            for (var i = 0; i < normalSyntax.Count; i++)
            {
                var left = normalSyntax[i];
                var right = dynamicSyntax[i];

                Assert.AreEqual(left.TheClientId, right.GetDynamicPropertyValue("TheClientId"));
                Assert.AreEqual(left.Count, right.GetDynamicPropertyValue("Count"));
                Assert.AreEqual(left.LongCount, right.GetDynamicPropertyValue("LongCount"));
                Assert.AreEqual(left.TaxAverage, right.GetDynamicPropertyValue("TaxAverage"));
                QueryableAssert.AreEqual(left.Sales.AsQueryable(), right.GetDynamicPropertyValue<List<MockSale>>("Sales").AsQueryable());
            }
        }

        [TestMethod]
        public void GroupWithoutNullCheckComplex()
        {
            var limitResult = TestData.Authors.Where(t => t.Posts != null).AsQueryable();

            var posts = limitResult
                .GroupBy(t => new
                {
                    Titles = t.Posts.Select(t2 => t2.Title)
                })
                .Select(t => new
                {
                    Titles = t.Key.Titles,
                    Data = t.ToList()
                })
                .ToList();

            var posts2 = limitResult
                .GroupBy(gb => gb.Path("Posts.Title", "Titles"))
                .Select(sb =>
                {
                    sb.Key("Titles");
                    sb.ToList("Data");
                })
                .ToDynamicClassList();

            Assert.AreEqual(posts.Count, posts2.Count);
            for(var i  = 0; i < posts.Count; i++)
            {
                var expected = posts[0];
                var actual = posts2[0];

                var titles = actual.GetDynamicPropertyValue("Titles") as ICollection;

                CollectionAssert.AreEqual(expected.Titles as ICollection, titles);
            }
        }

        [TestMethod]
        public void GroupWithNullCheckComplex()
        {
            var limitResult = TestData.Authors.AsQueryable();

            var posts = limitResult
                .GroupBy(t => new
                {
                    Titles = t.Posts == null ? new List<string>() : t.Posts.Select(t2 => t2.Title)
                })
                .Select(t => new
                {
                    Titles = t.Key.Titles,
                    Data = t.ToList()
                })
                .ToList();

            var tempQueryable = limitResult
                .GroupBy(gb => gb.NullChecking().Path("Posts.Title", "Titles"));


            var posts2 = tempQueryable 
                .Select(sb =>
                {
                    sb.Key("Titles");
                    sb.ToList("Data");
                })
                .ToDynamicClassList();

            Assert.AreEqual(posts.Count, posts2.Count);
            for (var i = 0; i < posts.Count; i++)
            {
                var expected = posts[0];
                var actual = posts2[0];

                var titles = actual.GetDynamicPropertyValue("Titles") as ICollection;

                CollectionAssert.AreEqual(expected.Titles as ICollection, titles);
            }
        }
    }
}
