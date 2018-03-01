using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq.Test.Helpers
{
    public static class QueryableAssert
    {
        private static bool _sameList<T>(IQueryable<T> a, IQueryable<T> b)
            where T : class
        {
            if (a.Count() != b.Count())
                return false;

            var listA = a.ToList();
            var listB = b.ToList();
            for (var i = 0; i < listA.Count; i++)
            {
                if (listA.ElementAt(i) != listB.ElementAt(i))
                    return false;
            }

            return true;
        }

        public static void AreEqual<T>(IQueryable<T> a, IQueryable<T> b)
            where T : class
        {
            Assert.IsTrue(_sameList(a, b));
        }

        public static void AreNotEqual<T>(IQueryable<T> a, IQueryable<T> b)
            where T : class
        {
            Assert.IsFalse(_sameList(a, b));
        }
    }
}
