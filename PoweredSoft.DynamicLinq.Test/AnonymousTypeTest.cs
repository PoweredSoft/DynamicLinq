using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoweredSoft.DynamicLinq.DynamicType;

namespace PoweredSoft.DynamicLinq.Test
{
    [TestClass]
    public class AnonymousTypeTest
    {
        [TestMethod]
        public void TestEqual()
        {
            var properties = new List<(Type type, string propertyName)>()
            {
                (typeof(int), "Id"),
                (typeof(string), "FirstName"),
                (typeof(string), "LastName")
            };

            var type = DynamicClassFactory.CreateType(properties);
            var instanceA = Activator.CreateInstance(type) as DynamicClass;
            var instanceB = Activator.CreateInstance(type) as DynamicClass;

            instanceA.SetDynamicPropertyValue("Id", 1);
            instanceA.SetDynamicPropertyValue("FirstName", "David");
            instanceA.SetDynamicPropertyValue("LastName", "Lebee");

            instanceB.SetDynamicPropertyValue("Id", 1);
            instanceB.SetDynamicPropertyValue("FirstName", "David");
            instanceB.SetDynamicPropertyValue("LastName", "Lebee");

            Assert.IsTrue(instanceA.Equals(instanceB));
        }
    }
}
