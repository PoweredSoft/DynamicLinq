using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PoweredSoft.DynamicLinq.Test
{
    [TestClass]
    public class CountTests
    {
        [TestMethod]
        public void Count()
        {
            var normalSyntax = TestData.Sales.Count();
            var nonGenericQueryable = (IQueryable)TestData.Sales.AsQueryable();
            var dynamicSyntax = nonGenericQueryable.Count();
            Assert.AreEqual(normalSyntax, dynamicSyntax);
        }

        [TestMethod]
        public void LongCount()
        {
            var normalSyntax = TestData.Sales.LongCount();
            var nonGenericQueryable = (IQueryable)TestData.Sales.AsQueryable();
            var dynamicSyntax = nonGenericQueryable.LongCount();
            Assert.AreEqual(normalSyntax, dynamicSyntax);
        }
    }
}
