using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Helpers;

namespace PoweredSoft.DynamicLinq.Test
{
    class Foo
    {

    }

    class ListOfFoo : List<Foo>
    {

    }

    [TestClass]
    public class HelpersTests
    {

        [TestMethod]
        public void TestInheritanceOfListAsGenericEnumerableType()
        {
            var shouldBeTrue = QueryableHelpers.IsGenericEnumerable(typeof(ListOfFoo));
            Assert.IsTrue(shouldBeTrue);
            var type = QueryableHelpers.GetTypeOfEnumerable(typeof(ListOfFoo), true);
            Assert.IsTrue(type == typeof(Foo));
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
                            Content = "ABC",
                            Comments = new List<Comment>()
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
                            Comments = new List<Comment>()
                        },
                        new Post
                        {
                            Id = 4,
                            AuthorId = 2,
                            Title = "DontMatch",
                            Content = "ASD",
                            Comments = new List<Comment>()
                        }
                    }
                }
            };

            // the query.
            var query = authors.AsQueryable();

            var allExpression = QueryableHelpers.CreateConditionExpression<Author>("Posts.Title", ConditionOperators.Equal, "Match", QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling.All);
            var anyExpression = QueryableHelpers.CreateConditionExpression<Author>("Posts.Title", ConditionOperators.Equal, "Match", QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling.Any);
            var anyExpression2 = QueryableHelpers.CreateConditionExpression<Author>("Posts.Comments.Email", ConditionOperators.Equal, "John.doe@me.com", QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling.Any);
            Assert.AreEqual(1, query.Count(allExpression));
            Assert.AreEqual(2, query.Count(anyExpression));
            Assert.AreEqual(1, query.Count(anyExpression2));
        }
    }
}
