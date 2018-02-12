using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Extensions;

namespace PoweredSoft.DynamicLinq.Test
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void Equal()
        {
            // subject.
            var authors = new List<Author>()
            {
                new Author { Id = long.MaxValue, FirstName = "David", LastName = "Lebee" }
            };

            // the query.
            var query = authors.AsQueryable();

            // simple where.
            var newQuery = query.Where("FirstName", ConditionOperators.Equal, "David");

            // must match.
            Assert.IsTrue(newQuery.Any(), "Must have at least one author that matches");
        }

        [TestMethod]
        public void Contains()
        {
            // subject.
            var authors = new List<Author>()
            {
                new Author { Id = long.MaxValue, FirstName = "David", LastName = "Lebee" }
            };

            // the query.
            var query = authors.AsQueryable();

            // simple where.
            var newQuery = query.Where("FirstName", ConditionOperators.Contains, "Da");

            // must match.
            Assert.IsTrue(newQuery.Any(), "Must have at least one author that matches");
        }

        [TestMethod]
        public void StartsWith()
        {
            // subject.
            var authors = new List<Author>()
            {
                new Author { Id = long.MaxValue, FirstName = "David", LastName = "Lebee" }
            };

            // the query.
            var query = authors.AsQueryable();

            // simple where.
            var newQuery = query.Where("FirstName", ConditionOperators.StartsWith, "Da");

            // must match.
            Assert.IsTrue(newQuery.Any(), "Must have at least one author that matches");
        }

        [TestMethod]
        public void EndsWith()
        {
            // subject.
            var authors = new List<Author>()
            {
                new Author { Id = long.MaxValue, FirstName = "David", LastName = "Lebee" }
            };

            // the query.
            var query = authors.AsQueryable();

            // simple where.
            var newQuery = query.Where("FirstName", ConditionOperators.EndsWith, "Da");

            // must match.
            Assert.IsFalse(newQuery.Any(), "Not suppose to find any matches");
        }

        [TestMethod]
        public void LessThen()
        {
            // subject.
            var authors = new List<Post>()
            {
                new Post { Id = 1, AuthorId = 1, Title = "Hello 1", Content = "World" },
                new Post { Id = 2, AuthorId = 1, Title = "Hello 2", Content = "World" },
                new Post { Id = 3, AuthorId = 2, Title = "Hello 3", Content = "World" },
            };

            // the query.
            var query = authors.AsQueryable();

            // simple where.
            var newQuery = query.Where("AuthorId", ConditionOperators.LessThan, 2);

            // must match.
            Assert.AreEqual(2, newQuery.Count());
        }

        [TestMethod]
        public void GreaterThanOrEqual()
        {
            // subject.
            var authors = new List<Post>()
            {
                new Post { Id = 1, AuthorId = 1, Title = "Hello 1", Content = "World" },
                new Post { Id = 2, AuthorId = 1, Title = "Hello 2", Content = "World" },
                new Post { Id = 3, AuthorId = 2, Title = "Hello 3", Content = "World" },
            };

            // the query.
            var query = authors.AsQueryable();

            // simple where.
            var newQuery = query.Where("AuthorId", ConditionOperators.GreaterThanOrEqual, 2);

            // must match.
            Assert.AreEqual(1, newQuery.Count());
        }
    }
}
