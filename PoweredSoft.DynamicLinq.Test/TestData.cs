using PoweredSoft.DynamicLinq.Dal.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Test
{
    internal class MockPersonObject
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }
    }

    internal class MockSale
    {
        public long Id { get; set; }
        public int ClientId { get; set; }
        public MockClient Client { get; set; }
        public decimal GrossSales { get; set; }
        public decimal NetSales { get; set; }
        public decimal Tax { get; set; }
    }

    internal class MockClient
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

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

        static readonly internal List<MockClient> Clients = new List<MockClient>
        {
            new MockClient { Id = 1, Name = "ACME INC."},
            new MockClient { Id = 2, Name = "MSLINK" },
            new MockClient { Id = 3, Name = "COOL GUYS TBD"},
            new MockClient { Id = 4, Name = "SOME LLC YEAH!" }
        };

        static readonly internal List<MockSale> Sales = new List<MockSale>
        {
            new MockSale { Id = 1, ClientId = 1, Client = Clients.First(t => t.Id == 1), GrossSales = 1000M, NetSales = 890.0M, Tax = 20M },
            new MockSale { Id = 2, ClientId = 1, Client = Clients.First(t => t.Id == 1), GrossSales = 1100M, NetSales = 180.0M, Tax = 0M },
            new MockSale { Id = 3, ClientId = 2, Client = Clients.First(t => t.Id == 2), GrossSales = 1200M, NetSales = 920.0M, Tax = 3M },
            new MockSale { Id = 4, ClientId = 2, Client = Clients.First(t => t.Id == 2), GrossSales = 1330M, NetSales = 800.0M, Tax = 120M },
            new MockSale { Id = 5, ClientId = 1, Client = Clients.First(t => t.Id == 1), GrossSales = 1400M, NetSales = 990.0M, Tax = 20M },
            new MockSale { Id = 6, ClientId = 3, Client = Clients.First(t => t.Id == 3), GrossSales = 1500M, NetSales = 290.0M, Tax = 200M },
            new MockSale { Id = 7, ClientId = 3, Client = Clients.First(t => t.Id == 3), GrossSales = 1600M, NetSales = 230.0M, Tax = 240M },
            new MockSale { Id = 8, ClientId = 3, Client = Clients.First(t => t.Id == 3), GrossSales = 1700M, NetSales = 330.0M, Tax = 210M },
            new MockSale { Id = 9, ClientId = 1, Client = Clients.First(t => t.Id == 1), GrossSales = 1800M, NetSales = 890.0M, Tax = 290M },
            new MockSale { Id = 10, ClientId = 4, Client = Clients.First(t => t.Id == 4), GrossSales = 1900M, NetSales = 490.0M, Tax = 270M }
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

        static readonly internal List<CommentLike> Likes = new List<CommentLike>()
        {
            new CommentLike
            {
                Id = 1,
                CommentId = 1,
                Comment = new Comment
                {
                    Email = "john@doe.ca",
                    CommentText = "bla bla",
                    Post = new Post
                    {
                        Content = "ASDFSADF"
                    }
                }
            },
            new CommentLike
            {
                Id = 2,
                CommentId = 2
            }
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
                                Email = "john.doe@me.com",
                                CommentLikes = new List<CommentLike>()
                                {
                                    new CommentLike()
                                    {
                                        Id = 1,
                                        CommentId = 1,
                                        CreateTime = DateTimeOffset.Now
                                    },
                                    new CommentLike()
                                    {
                                        Id = 2,
                                        CommentId = 1,
                                        CreateTime = DateTimeOffset.Now
                                    },
                                }
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
            },
            new Author
            {
                Id = 3,
                FirstName = "Mark",
                LastName = "Ronson"
            }
        };
    }
}
