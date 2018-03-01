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

            var q2 = Persons.AsQueryable().Query(t => t.GreaterThan("Age", 28).AndGreaterThan("Age", 50));
            var q2b = Persons.AsQueryable().Where(t => t.Age > 28 && t.Age > 50);
            QueryableAssert.AreEqual(q2, q2b);

            var q3 = Persons.AsQueryable().Query(t => t.GreaterThan("Age", 28).OrGreaterThan("Age", 50));
            var q3b = Persons.AsQueryable().Where(t => t.Age > 28 || t.Age > 50);
            QueryableAssert.AreEqual(q3, q3b);
        }

        [TestMethod]
        public void GreatThanOrEqual()
        {
            var q1 = Persons.AsQueryable().Query(t => t.GreaterThanOrEqual("Age", 28));
            var q1b = Persons.AsQueryable().Where(t => t.Age >= 28);
            QueryableAssert.AreEqual(q1, q1b);

            var q2 = Persons.AsQueryable().Query(t => t.GreaterThanOrEqual("Age", 28).AndGreaterThanOrEqual("Age", 50));
            var q2b = Persons.AsQueryable().Where(t => t.Age >= 28 && t.Age >= 50);
            QueryableAssert.AreEqual(q2, q2b);

            var q3 = Persons.AsQueryable().Query(t => t.GreaterThanOrEqual("Age", 28).OrGreaterThanOrEqual("Age", 50));
            var q3b = Persons.AsQueryable().Where(t => t.Age >= 28 || t.Age >= 50);
            QueryableAssert.AreEqual(q3, q3b);                  
        }

        [TestMethod]
        public void LessThan()
        {
            var q1 = Persons.AsQueryable().Query(t => t.LessThan("Age", 50));
            var q1b = Persons.AsQueryable().Where(t => t.Age < 50);
            QueryableAssert.AreEqual(q1, q1b);

            var q2 = Persons.AsQueryable().Query(t => t.LessThan("Age", 28).AndLessThan("Age", 50));
            var q2b = Persons.AsQueryable().Where(t => t.Age < 28 && t.Age < 50);
            QueryableAssert.AreEqual(q2, q2b);

            var q3 = Persons.AsQueryable().Query(t => t.LessThan("Age", 28).OrLessThan("Age", 50));
            var q3b = Persons.AsQueryable().Where(t => t.Age < 28 || t.Age < 50);
            QueryableAssert.AreEqual(q3, q3b);
        }

        [TestMethod]
        public void LessThanOrEqual()
        {
            var q1 = Persons.AsQueryable().Query(t => t.LessThanOrEqual("Age", 50));
            var q1b = Persons.AsQueryable().Where(t => t.Age <= 50);
            QueryableAssert.AreEqual(q1, q1b);

            var q2 = Persons.AsQueryable().Query(t => t.LessThanOrEqual("Age", 28).AndLessThanOrEqual("Age", 50));
            var q2b = Persons.AsQueryable().Where(t => t.Age <= 28 && t.Age <= 50);
            QueryableAssert.AreEqual(q2, q2b);

            var q3 = Persons.AsQueryable().Query(t => t.LessThanOrEqual("Age", 28).OrLessThanOrEqual("Age", 50));
            var q3b = Persons.AsQueryable().Where(t => t.Age <= 28 || t.Age <= 50);
            QueryableAssert.AreEqual(q3, q3b);
        }

        [TestMethod]
        public void StartsWith()
        {
            var q1 = Persons.AsQueryable().Query(t => t.StartsWith("FirstName", "Mi"));
            var q1b = Persons.AsQueryable().Where(t => t.FirstName.StartsWith("Mi"));
            QueryableAssert.AreEqual(q1, q1b);

            var q2 = Persons.AsQueryable().Query(t => t.StartsWith("FirstName", "Mi").AndStartsWith("LastName", "Vi"));
            var q2b = Persons.AsQueryable().Where(t => t.FirstName.StartsWith("Mi") && t.LastName.StartsWith("Vi"));
            QueryableAssert.AreEqual(q2, q2b);

            var q3 = Persons.AsQueryable().Query(t => t.StartsWith("FirstName", "Mi").OrStartsWith("FirstName", "Da"));
            var q3b = Persons.AsQueryable().Where(t => t.FirstName.StartsWith("Mi") || t.FirstName.StartsWith("Da"));
            QueryableAssert.AreEqual(q3, q3b);
        }

        [TestMethod]
        public void EndsWith()
        {
            var q1 = Persons.AsQueryable().Query(t => t.EndsWith("LastName", "ee"));
            var q1b = Persons.AsQueryable().Where(t => t.LastName.EndsWith("ee"));
            QueryableAssert.AreEqual(q1, q1b);

            var q2 = Persons.AsQueryable().Query(t => t.EndsWith("LastName", "ee").AndEndsWith("FirstName", "vid"));
            var q2b = Persons.AsQueryable().Where(t => t.LastName.EndsWith("ee") && t.FirstName.EndsWith("vid"));
            QueryableAssert.AreEqual(q2, q2b);

            var q3 = Persons.AsQueryable().Query(t => t.EndsWith("LastName", "ee").OrEndsWith("LastName", "ar"));
            var q3b = Persons.AsQueryable().Where(t => t.LastName.EndsWith("ee") || t.LastName.EndsWith("ar"));
            QueryableAssert.AreEqual(q3, q3b);
        }

        [TestMethod]
        public void Contains()
        {
            var q1 = Persons.AsQueryable().Query(t => t.Contains("LastName", "ee"));
            var q1b = Persons.AsQueryable().Where(t => t.LastName.Contains("ee"));
            QueryableAssert.AreEqual(q1, q1b);

            var q2 = Persons.AsQueryable().Query(t => t.Contains("LastName", "ee").AndContains("FirstName", "vid"));
            var q2b = Persons.AsQueryable().Where(t => t.LastName.Contains("ee") && t.FirstName.Contains("vid"));
            QueryableAssert.AreEqual(q2, q2b);

            var q3 = Persons.AsQueryable().Query(t => t.Contains("LastName", "ee").OrContains("LastName", "ar"));
            var q3b = Persons.AsQueryable().Where(t => t.LastName.Contains("ee") || t.LastName.Contains("ar"));
            QueryableAssert.AreEqual(q3, q3b);
        }
    }
}
