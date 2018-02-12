using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Extensions;
using PoweredSoft.DynamicLinq.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Test
{
    [TestClass]
    public class ComplexQueryTest
    {
        [TestMethod]
        public void ComplexQueryBuilder()
        {
            // subject.
            var posts = new List<Post>()
            {
                new Post { Id = 1, AuthorId = 1, Title = "Hello 1", Content = "World" },
                new Post { Id = 2, AuthorId = 1, Title = "Hello 2", Content = "World" },
                new Post { Id = 3, AuthorId = 2, Title = "Hello 3", Content = "World" },
            };

            // the query.
            var query = posts.AsQueryable();

            query = query.Query(q =>
            {
                q.Compare("AuthorId", ConditionOperators.Equal, 1);
                q.And(sq =>
                {
                    sq.Compare("Content", ConditionOperators.Equal, "World");
                    sq.Or("Title", ConditionOperators.Contains, 3);
                });
            });

            Assert.AreEqual(2, query.Count());
        }

        [TestMethod]
        public void UsingQueryBuilder()
        {
            // subject.
            var posts = new List<Post>()
            {
                new Post { Id = 1, AuthorId = 1, Title = "Hello 1", Content = "World" },
                new Post { Id = 2, AuthorId = 1, Title = "Hello 2", Content = "World" },
                new Post { Id = 3, AuthorId = 2, Title = "Hello 3", Content = "World" },
            };

            // the query.
            var query = posts.AsQueryable();
            var queryBuilder = new QueryBuilder<Post>(query);

            queryBuilder.Compare("AuthorId", ConditionOperators.Equal, 1);
            queryBuilder.And(subQuery =>
            {
                subQuery.Compare("Content", ConditionOperators.Equal, "World");
                subQuery.Or("Title", ConditionOperators.Contains, 3);
            });

            query = queryBuilder.Build();
            Assert.AreEqual(2, query.Count());
        }
    }
}
