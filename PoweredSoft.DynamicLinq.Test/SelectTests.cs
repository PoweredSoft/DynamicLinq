using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Test.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Test
{
    public class Mock
    {
        public int Id { get; set; }
        public int ForeignId { get; set; }
        public decimal Total { get; set; }

        public List<MockB> Bs { get; set; } = new List<MockB>();
    }

    public class MockB
    {
        public List<string> FirstNames { get; set; }
    }

    public class MockPerson
    {
        public string Name { get; set; }
        public MockListOfPhone Phones { get; set; }
    }

    public class MockPhone
    {
        public string Number { get; set; }
    }

    public class MockListOfPhone : List<MockPhone>
    {

    }

    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void TestSelectWithInheritedList()
        {
            var list = new List<MockPerson>()
            {
                new MockPerson
                {
                    Name = "David Lebee",
                    Phones = new MockListOfPhone
                    {
                        new MockPhone
                        {
                            Number = "0000000000"
                        }
                    }
                },
                new MockPerson
                {
                    Name = "Yubing Liang",
                    Phones = new MockListOfPhone
                    {
                        new MockPhone
                        {
                            Number = "1111111111"
                        }
                    }
                }
            };

            var names = list.AsQueryable()
                .Where(t => t.Equal("Phones.Number", "1111111111"))
                .Select(t =>
                {
                    t.Path("Name");
                    t.FirstOrDefault("Phones.Number", "Number", SelectCollectionHandling.Flatten);
                })
                .ToDynamicClassList();

            Assert.IsTrue(names.Count() == 1);
            var firstPerson = names.First();
            Assert.AreEqual("Yubing Liang", firstPerson.GetDynamicPropertyValue<string>("Name"));
            Assert.AreEqual("1111111111", firstPerson.GetDynamicPropertyValue<string>("Number"));
        }

        [TestMethod]
        public void TestSelect()
        {
            var list = new List<Mock>()
            {
                new Mock{
                    Id  = 1,
                    ForeignId = 1,
                    Total = 100,
                    Bs = new List<MockB>() {
                        new MockB { FirstNames =  new List<string>{"David", "John" } }
                    }
                },
            };

            var regularSyntaxA = list
                .AsQueryable()
                .Select(t => new
                {
                    Id = t.Id,
                    FirstNames = t.Bs.SelectMany(t2 => t2.FirstNames).ToList(),
                    FirstNamesLists = t.Bs.Select(t2 => t2.FirstNames).ToList(),
                    FirstFirstName = t.Bs.SelectMany(t2 => t2.FirstNames).First(),
                    FirstOrDefaultFirstName = t.Bs.SelectMany(t2 => t2.FirstNames).FirstOrDefault(),
                    LastFirstName = t.Bs.SelectMany(t2 => t2.FirstNames).Last(),
                    LastOrDefaultFirstName = t.Bs.SelectMany(t2 => t2.FirstNames).LastOrDefault(),

                    FirstFirstNameList = t.Bs.Select(t2 => t2.FirstNames).First(),
                    FirstOrDefaultFirstNameList = t.Bs.Select(t2 => t2.FirstNames).FirstOrDefault(),
                    LastFirstNameList = t.Bs.Select(t2 => t2.FirstNames).Last(),
                    LastOrDefaultFirstNameList = t.Bs.Select(t2 => t2.FirstNames).LastOrDefault()
                });
            
            var regularSyntax = regularSyntaxA.ToList();

            var dynamicSyntax = list
                .AsQueryable()
                .Select(t =>
                {
                    t.Path("Id");
                    t.ToList("Bs.FirstNames", "FirstNames", SelectCollectionHandling.Flatten);
                    t.ToList("Bs.FirstNames", "FirstNamesLists", SelectCollectionHandling.LeaveAsIs);
                    t.First("Bs.FirstNames", "FirstFirstName", SelectCollectionHandling.Flatten);
                    t.FirstOrDefault("Bs.FirstNames", "FirstOrDefaultFirstName", SelectCollectionHandling.Flatten);
                    t.Last("Bs.FirstNames", "LastFirstName", SelectCollectionHandling.Flatten);
                    t.LastOrDefault("Bs.FirstNames", "LastOrDefaultFirstName", SelectCollectionHandling.Flatten);

                    t.First("Bs.FirstNames", "FirstFirstNameList", SelectCollectionHandling.LeaveAsIs);
                    t.FirstOrDefault("Bs.FirstNames", "FirstOrDefaultFirstNameList", SelectCollectionHandling.LeaveAsIs);
                    t.Last("Bs.FirstNames", "LastFirstNameList", SelectCollectionHandling.LeaveAsIs);
                    t.LastOrDefault("Bs.FirstNames", "LastOrDefaultFirstNameList", SelectCollectionHandling.LeaveAsIs);
                })
                .ToDynamicClassList();

            Assert.AreEqual(regularSyntax.Count, dynamicSyntax.Count);
            for(var i = 0; i < regularSyntax.Count; i++)
            {
                Assert.AreEqual(regularSyntax[i].Id, dynamicSyntax[i].GetDynamicPropertyValue<int>("Id"));
                Assert.AreEqual(regularSyntax[i].FirstFirstName, dynamicSyntax[i].GetDynamicPropertyValue<string>("FirstFirstName"));
                Assert.AreEqual(regularSyntax[i].FirstOrDefaultFirstName, dynamicSyntax[i].GetDynamicPropertyValue<string>("FirstOrDefaultFirstName"));
                Assert.AreEqual(regularSyntax[i].LastFirstName, dynamicSyntax[i].GetDynamicPropertyValue<string>("LastFirstName"));
                Assert.AreEqual(regularSyntax[i].LastOrDefaultFirstName, dynamicSyntax[i].GetDynamicPropertyValue<string>("LastOrDefaultFirstName"));

                CollectionAssert.AreEqual(regularSyntax[i].FirstFirstNameList, dynamicSyntax[i].GetDynamicPropertyValue<List<string>>("FirstFirstNameList"));
                CollectionAssert.AreEqual(regularSyntax[i].FirstOrDefaultFirstNameList, dynamicSyntax[i].GetDynamicPropertyValue<List<string>>("FirstOrDefaultFirstNameList"));
                CollectionAssert.AreEqual(regularSyntax[i].LastFirstNameList, dynamicSyntax[i].GetDynamicPropertyValue<List<string>>("LastFirstNameList"));
                CollectionAssert.AreEqual(regularSyntax[i].LastOrDefaultFirstNameList, dynamicSyntax[i].GetDynamicPropertyValue<List<string>>("LastOrDefaultFirstNameList"));


                QueryableAssert.AreEqual(regularSyntax[i].FirstNames.AsQueryable(), dynamicSyntax[i].GetDynamicPropertyValue<List<string>>("FirstNames").AsQueryable());


                

                var left = regularSyntax[i].FirstNamesLists;
                var right = dynamicSyntax[i].GetDynamicPropertyValue<List<List<string>>>("FirstNamesLists");
                Assert.AreEqual(left.Count, right.Count);
                for(var j = 0; j < left.Count; j++)
                    QueryableAssert.AreEqual(left[j].AsQueryable(), right[j].AsQueryable());
            }
        }

        [TestMethod]
        public void SelectNullChecking()
        {
            var query = TestData.Authors.AsQueryable();

            
            var qs = query.Select(t => new
            {
                CommentLikes = t.Posts == null ? 
                    new List<CommentLike>() :
                    t.Posts.Where(t2 => t2.Comments != null).SelectMany(t2 => t2.Comments.Where(t3 => t3.CommentLikes != null).SelectMany(t3 => t3.CommentLikes)).ToList()
            });

            var a = qs.ToList();

            var querySelect = query.Select(t =>
            {
                t.NullChecking(true);
                t.ToList("Posts.Comments.CommentLikes", selectCollectionHandling: SelectCollectionHandling.Flatten);
            });

            var b = querySelect.ToDynamicClassList();

            Assert.AreEqual(a.Count, b.Count);
            for(var i = 0; i  < a.Count; i++)
            {
                var left = a[i];
                var right = b[i];

                var leftCommentLikes = left.CommentLikes;
                var rightCommentLikes = right.GetDynamicPropertyValue<List<CommentLike>>("CommentLikes");
                QueryableAssert.AreEqual(leftCommentLikes.AsQueryable(), rightCommentLikes.AsQueryable());
            }
        }

        [TestMethod]
        public void SelectNullChecking2()
        {
            var query = TestData.Likes.AsQueryable();

            var qs = query.Select(t => new
            {
                Post = t.Comment == null || t.Comment.Post == null ? null : t.Comment.Post,
                Texts = (t.Comment == null || t.Comment.Post == null || t.Comment.Post.Comments == null ? new List<string>() : t.Comment.Post.Comments.Select(t2 => t2.CommentText)).ToList()
            });

            var a = qs.ToList();

            var querySelect = query.Select(t =>
            {
                t.NullChecking(true);
                // this needs to be fixed.
                t.Path("Comment.CommentText", "CommentText2");
                //t.PathToList("Comment.Post.Comments.CommentText", selectCollectionHandling: SelectCollectionHandling.Flatten);
            });

            var b = querySelect.ToDynamicClassList();
        }
    }
}
