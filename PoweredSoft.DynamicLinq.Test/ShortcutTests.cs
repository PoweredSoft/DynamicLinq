using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq.Extensions;
using PoweredSoft.DynamicLinq.Test.Helpers;

namespace PoweredSoft.DynamicLinq.Test
{
    internal class MockPersonObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }
    }

    [TestClass]
    public class ShortcutTests
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
        public void Equal()
        {
            // test simple
            var q1 = Persons.AsQueryable().Query(t => t.Equal("FirstName", "David"));
            var q1b = Persons.AsQueryable().Where(t => t.FirstName == "David");
            QueryableAssert.AreEqual(q1, q1b);

            // test and
            var q2 = Persons.AsQueryable().Query(t => t.Equal("FirstName", "David").AndEqual("LastName", "Lebee"));
            var q2b = Persons.AsQueryable().Where(t => t.FirstName == "David" && t.LastName == "Lebee");
            QueryableAssert.AreEqual(q2, q2b);


            // test or
            var q3 = Persons.AsQueryable().Query(t => t.Equal("FirstName", "David").OrEqual("FirstName", "Michaela"));
            var q3b = Persons.AsQueryable().Where(t => t.FirstName == "David" || t.FirstName == "Michaela");
            QueryableAssert.AreEqual(q3, q3b);
        }

        [TestMethod]
        public void NotEqual()
        {
            // test simple
            var q1 = Persons.AsQueryable().Query(t => t.NotEqual("FirstName", "David"));
            var q1b = Persons.AsQueryable().Where(t => t.FirstName != "David");
            QueryableAssert.AreEqual(q1, q1b);

            // test and
            var q2 = Persons.AsQueryable().Query(t => t.NotEqual("FirstName", "David").AndNotEqual("FirstName", "Michaela"));
            var q2b = Persons.AsQueryable().Where(t => t.FirstName != "David" && t.FirstName != "Michaela");
            QueryableAssert.AreEqual(q2, q2b);

            // test or
            var q3 = Persons.AsQueryable().Query(t => t.NotEqual("FirstName", "David").OrNotEqual("LastName", "Lebee"));
            var q3b = Persons.AsQueryable().Where(t => t.FirstName != "David" || t.LastName != "Lebee");
            QueryableAssert.AreEqual(q3, q3b);
        }

        [TestMethod]
        public void GreatThan()
        {
            var q1 = Persons.AsQueryable().Query(t => t.GreaterThan("Age", 28));
            var q1b = Persons.AsQueryable().Where(t => t.Age > 28);
            QueryableAssert.AreEqual(q1, q1b);
        }

        [TestMethod]
        public void GreatThanOrEqual()
        {

        }

        [TestMethod]
        public void LessThan()
        {

        }

        [TestMethod]
        public void LessThanOrEqual()
        {

        }

        [TestMethod]
        public void StartsWith()
        {

        }

        [TestMethod]
        public void EndsWith()
        {

        }

        [TestMethod]
        public void Contains()
        {

        }
    }
}
