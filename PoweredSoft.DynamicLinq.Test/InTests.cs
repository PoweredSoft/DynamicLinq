using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq;
using PoweredSoft.DynamicLinq.Test.Helpers;

namespace PoweredSoft.DynamicLinq.Test
{
    [TestClass]
    public class InTests
    {
        internal List<MockPersonObject> Persons = new List<MockPersonObject>
        {
            new MockPersonObject { FirstName = "David", LastName = "Lebee", Age = 28 },
            new MockPersonObject { FirstName = "Michaela", LastName = "Vickar", Age = 27 },
            new MockPersonObject { FirstName = "John", LastName = "Doe", Age = 28 },
            new MockPersonObject { FirstName = "Chuck", LastName = "Norris", Age = 50 },
            new MockPersonObject { FirstName = "Michael", LastName = "Jackson", Age = 58 }
        };

        [TestMethod]
        public void In()
        {
            IQueryable<MockPersonObject> a, b;
            var ageGroup = new List<int>() { 28, 27, 50 };

            a = Persons.AsQueryable().Query(t => t.In("Age", ageGroup));
            b = Persons.AsQueryable().Where(t => ageGroup.Contains(t.Age));
            QueryableAssert.AreEqual(a, b);
        }

        [TestMethod]
        public void NotIn()
        {
            IQueryable<MockPersonObject> a, b;
            var ageGroup = new List<int>() { 50, 58 };
            a = Persons.AsQueryable().Query(t => t.NotIn("Age", ageGroup));
            b = Persons.AsQueryable().Where(t => !ageGroup.Contains(t.Age));
            QueryableAssert.AreEqual(a, b);
        }

        [TestMethod]
        public void InString()
        {
            IQueryable<MockPersonObject> a, b;
            var group = new List<string>() { "David", "Michaela" };
            a = Persons.AsQueryable().Query(t => t.In("FirstName", group));
            b = Persons.AsQueryable().Where(t => group.Contains(t.FirstName));
            QueryableAssert.AreEqual(a, b);
        }

        [TestMethod]
        public void DiffTypeListConversion()
        {
            IQueryable<MockPersonObject> a, b;
            var ageGroup = new List<string>() { "28", "27", "50" };
            var ageGroupInt = ageGroup.Select(t => Convert.ToInt32(t)).ToList();

            a = Persons.AsQueryable().Query(t => t.In("Age", ageGroup));
            b = Persons.AsQueryable().Where(t => ageGroupInt.Contains(t.Age));
            QueryableAssert.AreEqual(a, b);
        }
    }
}
