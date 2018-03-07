using PoweredSoft.DynamicLinq.Dal.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Test
{
    internal static class TestData
    {
        static readonly internal List<MockPersonObject> Persons = new List<MockPersonObject>
        {
            new MockPersonObject { FirstName = "David", LastName = "Lebee", Age = 28 },
            new MockPersonObject { FirstName = "Michaela", LastName = "Vickar", Age = 27 },
            new MockPersonObject { FirstName = "John", LastName = "Doe", Age = 28 },
            new MockPersonObject { FirstName = "Chuck", LastName = "Norris", Age = 50 },
            new MockPersonObject { FirstName = "Michael", LastName = "Jackson", Age = 58 }
        };

        static readonly internal List<Post> Posts = new List<Post>()
        {
            new Post
            {
                Id = 1,
                Author = new Author()
                {
                    Id = 1,
                    FirstName = "David",
                    LastName = "Lebee"
                },
                AuthorId = 1,
                CreateTime = DateTime.Now,
                Title = "Match",
                Content = "ABC",
            },
            new Post
            {
                Id = 2,
                Author = new Author()
                {
                    Id = 1,
                    FirstName = "David",
                    LastName = "Lebee"
                },
                AuthorId = 1,
                CreateTime = DateTime.Now,
                Title = "Match 2",
                Content = "ABC 2",
            },
            new Post
            {
                Id = 3,
                Author = new Author()
                {
                    Id = 2,
                    FirstName = "John",
                    LastName = "Doe"
                },
                AuthorId = 3,
                CreateTime = DateTime.Now,
                Title = "Match 3",
                Content = "ABC 3",
            },
        };

        static readonly internal List<Author> Authors = new List<Author>()
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
                                Email = "john.doe@me.com"
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
                        Content = "ASD"
                    },
                    new Post
                    {
                        Id = 4,
                        AuthorId = 2,
                        Title = "DontMatch",
                        Content = "ASD"
                    }
                }
            }
        };
    }
}
