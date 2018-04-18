using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq.Dal.Pocos;

namespace PoweredSoft.DynamicLinq.Test
{
    internal class ConstantTestClass
    {
        public int Id { get; set; }
        public int? ForeignKey { get; set; }
        public string Text { get; set; }
    }

    [TestClass]
    public class ConstantTests
    {
        internal List<ConstantTestClass> Posts { get; set; } = new List<ConstantTestClass>()
        {
            new ConstantTestClass { Id = 1, ForeignKey = null, Text = "Hello" },
            new ConstantTestClass { Id = 2, ForeignKey = 1, Text = "Hello 2" },
            new ConstantTestClass { Id = 3, ForeignKey = 2, Text = "Hello 3" },
            new ConstantTestClass { Id = 4, ForeignKey = null, Text = "Hello 4" },
        };

        [TestMethod]
        public void LeaveAsIs()
        {
            try
            {
                Posts
                    .AsQueryable()
                    .Query(t => t.Equal("ForeignKey", 1, QueryConvertStrategy.LeaveAsIs));

                Assert.Fail("Should have thrown an exception");
            }
            catch
            {
            }

            Assert.IsTrue(Posts.AsQueryable().Query(t => t.Equal("Id", 1, QueryConvertStrategy.LeaveAsIs)).Any());
        }

        [TestMethod]
        public void TestGuid()
        {
            var randomGuidStr = Guid.NewGuid().ToString();
            TestData.Uniques.AsQueryable().Query(t => t.Equal("RowNumber", randomGuidStr));
            TestData.Uniques.AsQueryable().Query(t => t.Equal("OtherNullableGuid", randomGuidStr));
        }

        [TestMethod]
        public void SpecifyType()
        {
            Assert.IsTrue(Posts.AsQueryable().Query(t => t.Equal("ForeignKey", 1, QueryConvertStrategy.SpecifyType)).Any());
            Assert.IsTrue(Posts.AsQueryable().Query(t => t.Equal("Id", 1, QueryConvertStrategy.SpecifyType)).Any());

            try
            {
                Posts.AsQueryable().Query(t => t.Equal("Id", "1", QueryConvertStrategy.SpecifyType));
                Assert.Fail("Should have thrown an exception");
            }
            catch
            {

            }
        }

        [TestMethod]
        public void ConvertConstantToComparedPropertyOrField()
        {
            Assert.IsTrue(Posts.AsQueryable().Query(t => t.Equal("ForeignKey", 1, QueryConvertStrategy.ConvertConstantToComparedPropertyOrField)).Any());
            Assert.IsTrue(Posts.AsQueryable().Query(t => t.Equal("ForeignKey", "1", QueryConvertStrategy.ConvertConstantToComparedPropertyOrField)).Any());
            Assert.IsTrue(Posts.AsQueryable().Query(t => t.Equal("Id", 1, QueryConvertStrategy.ConvertConstantToComparedPropertyOrField)).Any());
            Assert.IsTrue(Posts.AsQueryable().Query(t => t.Equal("Id", "1", QueryConvertStrategy.ConvertConstantToComparedPropertyOrField)).Any());
        }
    }
}
