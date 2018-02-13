using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PoweredSoft.DynamicLinq
{
    public enum ConditionOperators 
    {
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith
    }

    public enum QueryConvertStrategy
    {
        LeaveAsIs,
        ConvertConstantToComparedPropertyOrField
    }

    public enum QueryCollectionCondition
    {
        Any,
        All
    }

    internal static class Constants
    {
        internal static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains");
        internal static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        internal static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
        internal static readonly MethodInfo AnyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(t => t.Name == "Any" && t.GetParameters().Count() == 2);
        internal static readonly MethodInfo AllMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(t => t.Name == "All" && t.GetParameters().Count() == 2);
    }
}
