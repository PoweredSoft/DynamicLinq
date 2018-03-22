using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq.Dal.Pocos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Test
{
    [TestClass]
    public class SelectTests
    {
        [TestMethod]
        public void TestSelect()
        {
            var regularSyntaxA = TestData.Authors
                .AsQueryable()
                .Select(t => new
                {
                    Id = t.Id,
                    AuthorFirstName = t.FirstName,
                    AuthorLastName = t.LastName,
                    Posts = t.Posts.ToList(),
                    Comments = t.Posts.Select(t2 => t2.Comments).ToList(),
                    Comments2 = t.Posts.SelectMany(t2 => t2.Comments).ToList()
                });
            
            //var regularSyntax = regularSyntaxA.ToList();

            var dynamicSyntax = TestData.Authors
                .AsQueryable()
                .Select(t =>
                {
                    t.Path("Id");
                    t.Path("FirstName", "AuthorFirstName");
                    t.Path("LastName", "AuthorLastName");
                    t.PathToList("Posts");
                    t.PathToList("Posts.Comments");
                    t.PathToList("Posts.Comments", "Comments2", SelectCollectionHandling.SelectMany);
                })
                .ToDynamicClassList();

            /*
            Assert.AreEqual(regularSyntax.Count, dynamicSyntax.Count);
            for(var i = 0; i < regularSyntax.Count; i++)
            {
                Assert.AreEqual(regularSyntax[i].Id, dynamicSyntax[i].GetDynamicPropertyValue<long>("Id"));
                Assert.AreEqual(regularSyntax[i].AuthorFirstName, dynamicSyntax[i].GetDynamicPropertyValue<string>("AuthorFirstName"));
                Assert.AreEqual(regularSyntax[i].AuthorLastName, dynamicSyntax[i].GetDynamicPropertyValue<string>("AuthorLastName"));
                Helpers.QueryableAssert.AreEqual(regularSyntax[i].Posts.AsQueryable(), dynamicSyntax[i].GetDynamicPropertyValue<List<Post>>("Posts").AsQueryable());
                //Helpers.QueryableAssert.AreEqual(regularSyntax[i].Comments.AsQueryable(), dynamicSyntax[i].GetDynamicPropertyValue<List<Comment>>("Comments").AsQueryable());
            }*/
        }

        [TestMethod]
        public void SelectNullChecking()
        {
            // TODO: null checking in select operations :D
            throw new Exception("TODO");
        }
    }
}
