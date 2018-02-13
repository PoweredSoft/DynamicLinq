using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Extensions;
using PoweredSoft.DynamicLinq.Fluent;
using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        [TestMethod]
        public void TestingSort()
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

            // add some sorting.
            queryBuilder
                .OrderByDescending("AuthorId")
                .ThenBy("Id");

            query = queryBuilder.Build();

            var first = query.First();
            var second = query.Skip(1).First();

            Assert.IsTrue(first.Id == 3);
            Assert.IsTrue(second.Id == 1);
        }

        [TestMethod]
        public void TestCreateFilterExpression()
        {
            var authors = new List<Author>()
            {
                new Author
                {
                    Id = 1,
                    FirstName = "David",
                    LastName = "Lebee",
                    Posts = new List<Post>
                    {
                        new Post
                        {
                            Id = 1,
                            AuthorId = 1,
                            Title = "Match",
                            Content = "ABC",
                            Comments = new List<Comment>()
                            {
                                new Comment()
                                {
                                    Id = 1,
                                    DisplayName = "John Doe",
                                    CommentText = "!@#$!@#!@#",
                                    Email = "John.doe@me.com"
                                }
                            }
                        },
                        new Post
                        {
                            Id = 2,
                            AuthorId = 1,
                            Title = "Match",
                            Content = "ABC"
                        }
                    }
                },
                new Author
                {
                    Id = 2,
                    FirstName = "Chuck",
                    LastName = "Norris",
                    Posts = new List<Post>
                    {
                        new Post
                        {
                            Id = 3,
                            AuthorId = 2,
                            Title = "Match",
                            Content = "ASD",
                        },
                        new Post
                        {
                            Id = 4,
                            AuthorId = 2,
                            Title = "DontMatch",
                            Content = "ASD",
                        }
                    }
                }
            };

            // the query.
            var query = authors.AsQueryable();

            var allExpression = QueryableHelpers.CreateFilterExpression<Author>("Posts.Title", ConditionOperators.Equal, "Match", QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionCondition.All);
            var anyExpression = QueryableHelpers.CreateFilterExpression<Author>("Posts.Title", ConditionOperators.Equal, "Match", QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionCondition.Any);
            Assert.AreEqual(1, query.Count(allExpression));
            Assert.AreEqual(2, query.Count(anyExpression));
        }
    }
}
