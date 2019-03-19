using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq;
using PoweredSoft.DynamicLinq.Test.Helpers;

namespace PoweredSoft.DynamicLinq.Test
{
    [TestClass]
    public class StringComparisionTests
    {
        internal List<MockPersonObject> Persons => TestData.Persons;

        [TestMethod]
        public void Equal()
        {
            IQueryable<MockPersonObject> a, b;

            // case sensitive.
            a = Persons.AsQueryable().Query(t => t.Equal("FirstName", "David", stringComparision: null));
            b = Persons.AsQueryable().Where(t => t.FirstName == "David");
            QueryableAssert.AreEqual(a, b, "CaseSensitive");

            // not case sensitive
            a = Persons.AsQueryable().Query(t => t.Equal("FirstName", "DAVID", stringComparision: StringComparison.OrdinalIgnoreCase));
            b = Persons.AsQueryable().Where(t => t.FirstName.Equals("DAVID", StringComparison.OrdinalIgnoreCase));
            QueryableAssert.AreEqual(a, b, "CaseInsensitive");
        }

        [TestMethod]
        public void NotEqual()
        {
            IQueryable<MockPersonObject> a, b;

            // case sensitive.
            a = Persons.AsQueryable().Query(t => t.NotEqual("FirstName", "David", stringComparision: null));
            b = Persons.AsQueryable().Where(t => t.FirstName != "David");
            QueryableAssert.AreEqual(a, b, "CaseSensitive");

            // not case sensitive
            a = Persons.AsQueryable().Query(t => t.NotEqual("FirstName", "DAVID", stringComparision: StringComparison.OrdinalIgnoreCase));
            b = Persons.AsQueryable().Where(t => !t.FirstName.Equals("DAVID", StringComparison.OrdinalIgnoreCase));
            QueryableAssert.AreEqual(a, b, "CaseInsensitive");
        }

        [TestMethod]
        public void Contains()
        {
            IQueryable<MockPersonObject> a, b;

            // case sensitive.
            a = Persons.AsQueryable().Query(t => t.Contains("FirstName", "vi", stringComparision: null));
            b = Persons.AsQueryable().Where(t => t.FirstName.Contains("vi"));
            QueryableAssert.AreEqual(a, b, "CaseSensitive");

            // not case sensitive
            a = Persons.AsQueryable().Query(t => t.Contains("FirstName", "VI", stringComparision: StringComparison.OrdinalIgnoreCase));
            b = Persons.AsQueryable().Where(t => t.FirstName.IndexOf("VI", StringComparison.OrdinalIgnoreCase) > -1);
            QueryableAssert.AreEqual(a, b, "CaseInsensitive");
        }

        [TestMethod]
        public void NotContains()
        {
            IQueryable<MockPersonObject> a, b;

            // case sensitive.
            a = Persons.AsQueryable().Query(t => t.NotContains("FirstName", "vi", stringComparision: null));
            b = Persons.AsQueryable().Where(t => !t.FirstName.Contains("vi"));
            QueryableAssert.AreEqual(a, b, "CaseSensitive");

            // not case sensitive
            a = Persons.AsQueryable().Query(t => t.NotContains("FirstName", "VI", stringComparision: StringComparison.OrdinalIgnoreCase));
            b = Persons.AsQueryable().Where(t => t.FirstName.IndexOf("VI", StringComparison.OrdinalIgnoreCase) == -1);
            QueryableAssert.AreEqual(a, b, "CaseInsensitive");
        }

        [TestMethod]
        public void StartsWith()
        {
            IQueryable<MockPersonObject> a, b;

            // case sensitive.
            a = Persons.AsQueryable().Query(t => t.StartsWith("FirstName", "Da", stringComparision: null));
            b = Persons.AsQueryable().Where(t => t.FirstName.StartsWith("Da"));
            QueryableAssert.AreEqual(a, b, "CaseSensitive");

            // not case sensitive
            a = Persons.AsQueryable().Query(t => t.StartsWith("FirstName", "DA", stringComparision: StringComparison.OrdinalIgnoreCase));
            b = Persons.AsQueryable().Where(t => t.FirstName.StartsWith("DA", StringComparison.OrdinalIgnoreCase));
            QueryableAssert.AreEqual(a, b, "CaseInsensitive");
        }

        [TestMethod]
        public void EndsWith()
        {
            IQueryable<MockPersonObject> a, b;

            // case sensitive.
            a = Persons.AsQueryable().Query(t => t.EndsWith("FirstName", "vid", stringComparision: null));
            b = Persons.AsQueryable().Where(t => t.FirstName.EndsWith("vid"));
            QueryableAssert.AreEqual(a, b, "CaseSensitive");

            // not case sensitive
            a = Persons.AsQueryable().Query(t => t.EndsWith("FirstName", "VID", stringComparision: StringComparison.OrdinalIgnoreCase));
            b = Persons.AsQueryable().Where(t => t.FirstName.EndsWith("VID", StringComparison.OrdinalIgnoreCase));
            QueryableAssert.AreEqual(a, b, "CaseInsensitive");
        }
    }
}
