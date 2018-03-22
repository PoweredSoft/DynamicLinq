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
            var regularSyntax = TestData.Authors
                .AsQueryable()
                .Select(t => new
                {
                    Id = t.Id,
                    AuthorFirstName = t.FirstName,
                    AuthorLastName = t.LastName,
                    Posts = t.Posts.ToList()
                }).ToList();

            var dynamicSyntax = TestData.Authors
                .AsQueryable()
                .Select(t =>
            {
                t.Path("Id");
                t.Path("FirstName", "AuthorFirstName");
                t.Path("LastName", "AuthorLastName");
                t.PathToList("Posts");
            }).ToDynamicClassList();

            Assert.AreEqual(regularSyntax.Count, dynamicSyntax.Count);
            for(var i = 0; i < regularSyntax.Count; i++)
            {
                Assert.AreEqual(regularSyntax[i].Id, dynamicSyntax[i].GetDynamicPropertyValue<long>("Id"));
                Assert.AreEqual(regularSyntax[i].AuthorFirstName, dynamicSyntax[i].GetDynamicPropertyValue<string>("AuthorFirstName"));
                Assert.AreEqual(regularSyntax[i].AuthorLastName, dynamicSyntax[i].GetDynamicPropertyValue<string>("AuthorLastName"));
                Helpers.QueryableAssert.AreEqual(regularSyntax[i].Posts.AsQueryable(), dynamicSyntax[i].GetDynamicPropertyValue<List<Post>>("Posts").AsQueryable());
            }
        }
    }
}
