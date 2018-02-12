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

        //[TestMethod]
        //public void Test()
        //{
        //    var authors = new List<Author>()
        //    {
        //        new Author
        //        {
        //            Id = 1,
        //            FirstName = "David",
        //            LastName = "Lebee",
        //            Posts = new List<Post>
        //            {
        //                new Post
        //                {
        //                    Id = 1,
        //                    AuthorId = 1,
        //                    Title = "Match",
        //                    Content = "ABC",
        //                    Comments = new List<Comment>()
        //                    {
        //                        new Comment()
        //                        {
        //                            Id = 1,
        //                            DisplayName = "John Doe",
        //                            CommentText = "!@#$!@#!@#",
        //                            Email = "John.doe@me.com"
        //                        }
        //                    }
        //                },
        //                new Post
        //                {
        //                    Id = 2,
        //                    AuthorId = 1,
        //                    Title = "Match",
        //                    Content = "ABC"
        //                }
        //            }
        //        },
        //        new Author
        //        {
        //            Id = 2,
        //            FirstName = "Chuck",
        //            LastName = "Norris",
        //            Posts = new List<Post>
        //            {
        //                new Post
        //                {
        //                    Id = 3,
        //                    AuthorId = 2,
        //                    Title = "Match",
        //                    Content = "ASD",
        //                },
        //                new Post
        //                {
        //                    Id = 4,
        //                    AuthorId = 2,
        //                    Title = "DontMatch",
        //                    Content = "ASD",
        //                }
        //            }
        //        }
        //    };

        //    // the query.
        //    var query = authors.AsQueryable();

        //    //// first recursion.
        //    //var typeOfClass = typeof(Author);
        //    //var parameter = Expression.Parameter(typeOfClass, "t");
        //    //var posts = Expression.PropertyOrField(parameter, "Posts");

        //    //// second recursion
        //    //{
        //    //    var subListType = posts.Type.GetGenericArguments().First();

        //    //    var innerParam = Expression.Parameter(subListType, "t2");
        //    //    var field = Expression.PropertyOrField(innerParam, "Title");
        //    //    var innerCondition = PoweredSoft.DynamicLinq.Helpers.QueryableHelpers.GetConditionExpressionForMember(innerParam, field, ConditionOperators.Equal, Expression.Constant("Match"));
        //    //    var lambda = Expression.Lambda(innerCondition, innerParam);

        //    //    // any
        //    //    var anyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(t => t.Name == "All" && t.GetParameters().Count() == 2);
        //    //    var genericAnyMethod = anyMethod.MakeGenericMethod(subListType);
        //    //    var subExpression = Expression.Call(genericAnyMethod, posts, lambda);

        //    //    var finalLambda = Expression.Lambda<Func<Author, bool>>(subExpression, parameter);
        //    //    query = query.Where(finalLambda);
        //    //}

        //    var anyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(t => t.Name == "Any" && t.GetParameters().Count() == 2);

        //    // first recursion.
        //    var typeOfClass = typeof(Author);
        //    var parameter = Expression.Parameter(typeOfClass, "t");
        //    var posts = Expression.PropertyOrField(parameter, "Posts");

        //    // second recursion
        //    {
        //        var subListType = posts.Type.GetGenericArguments().First();

        //        var innerParam = Expression.Parameter(subListType, "t2");
        //        var field = Expression.PropertyOrField(innerParam, "Comments");

        //        // any
        //        var genericAnyMethod = anyMethod.MakeGenericMethod(subListType);

        //        // third recursion
        //        {
        //            var innerParam2 = Expression.Parameter(typeof(Comment), "t3");
        //            var field2 = Expression.PropertyOrField(innerParam2, "Id");
        //            var innerCondition2 = QueryableHelpers.GetConditionExpressionForMember(innerParam2, field2, ConditionOperators.Equal, Expression.Constant(1L));
        //            var lambda = Expression.Lambda(innerCondition2, innerParam2);

        //            var generateAnyMethod2 = anyMethod.MakeGenericMethod(typeof(Comment));
        //            var subExpression2 = Expression.Call(generateAnyMethod2, field, lambda);

        //            var previousLambda = Expression.Lambda<Func<Post, bool>>(subExpression2, innerParam);

        //            var subExpression = Expression.Call(genericAnyMethod, posts, previousLambda);

        //            var finalLambda = Expression.Lambda<Func<Author, bool>>(subExpression, parameter);
        //            query = query.Where(finalLambda);
        //        }
        //    }
        //}
    }
}
