using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq;
using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Test.Helpers;

namespace PoweredSoft.DynamicLinq.Test
{
    [TestClass]
    public class InTests
    {
        [TestMethod]
        public void In()
        {
            IQueryable<MockPersonObject> a, b;
            var ageGroup = new List<int>() { 28, 27, 50 };
            a = TestData.Persons.AsQueryable().Query(t => t.In("Age", ageGroup));
            b = TestData.Persons.AsQueryable().Where(t => ageGroup.Contains(t.Age));
            QueryableAssert.AreEqual(a, b);
        }

        [TestMethod]
        public void NotIn()
        {
            IQueryable<MockPersonObject> a, b;
            var ageGroup = new List<int>() { 50, 58 };
            a = TestData.Persons.AsQueryable().Query(t => t.NotIn("Age", ageGroup));
            b = TestData.Persons.AsQueryable().Where(t => !ageGroup.Contains(t.Age));
            QueryableAssert.AreEqual(a, b);
        }

        [TestMethod]
        public void InString()
        {
            IQueryable<MockPersonObject> a, b;
            var group = new List<string>() { "David", "Michaela" };
            a = TestData.Persons.AsQueryable().Query(t => t.In("FirstName", group));
            b = TestData.Persons.AsQueryable().Where(t => group.Contains(t.FirstName));
            QueryableAssert.AreEqual(a, b);
        }

        [TestMethod]
        public void DiffTypeListConversion()
        {
            IQueryable<MockPersonObject> a, b;
            var ageGroup = new List<string>() { "28", "27", "50" };
            var ageGroupInt = ageGroup.Select(t => Convert.ToInt32(t)).ToList();

            a = TestData.Persons.AsQueryable().Query(t => t.In("Age", ageGroup));
            b = TestData.Persons.AsQueryable().Where(t => ageGroupInt.Contains(t.Age));
            QueryableAssert.AreEqual(a, b);
        }

        [TestMethod]
        public void MixingInWithCollectionPaths()
        {
            var titles = new List<string>() { "Match" };
            var a = TestData.Authors.AsQueryable().Query(t => t.In("Posts.Title", titles));
            var b = TestData.Authors.AsQueryable().Where(t => t.Posts.Any(t2 => titles.Contains(t2.Title)));
            QueryableAssert.AreEqual(a, b);
        }

        [TestMethod]
        public void MixingInComplexPaths()
        {
            var authorsFirstNames = new List<string>() { "David", "Pablo" };
            var a = TestData.Posts.AsQueryable().Query(t => t.In("Author.FirstName", authorsFirstNames));
            var b = TestData.Posts.AsQueryable().Where(t => authorsFirstNames.Contains(t.Author.FirstName));
            QueryableAssert.AreEqual(a, b);
        }
    }
}
