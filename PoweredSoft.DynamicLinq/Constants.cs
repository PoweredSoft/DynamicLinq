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
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith,
        In,
        NotIn
    }

    public enum QueryConvertStrategy
    {
        LeaveAsIs,
        ConvertConstantToComparedPropertyOrField,
        SpecifyType
    }

    public enum QueryCollectionHandling
    {
        Any,
        All
    }

    public enum QueryOrderByDirection
    {
        Ascending,
        Descending
    }

    public enum SelectTypes
    {
        Key,
        Count,
        LongCount,
        Sum,
        Average,
        ToList,
        PathToList,
        Path
    }

    public enum SelectCollectionHandling
    {
        LeaveAsIs,
        Flatten
    }

    internal static class Constants
    {
        internal static readonly MethodInfo GroupByMethod = typeof(Queryable).GetMethods().First(t => t.Name == "GroupBy");
        internal static readonly MethodInfo GroupByMethodWithEqualityComparer = typeof(Queryable).GetMethods().First(t => t.Name == "GroupBy" && t.GetParameters().Any(t2 => t2.Name == "comparer"));
        internal static readonly MethodInfo StringEqualWithComparisation = typeof(string).GetMethod("Equals", new Type[] { typeof(string), typeof(StringComparison) });
        internal static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        internal static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        internal static readonly MethodInfo StartsWithMethodWithComparisation = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string), typeof(StringComparison) });
        internal static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
        internal static readonly MethodInfo EndsWithMethodWithComparisation = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string), typeof(StringComparison) });
        internal static readonly MethodInfo IndexOfMethod = typeof(string).GetMethod("IndexOf", new Type[] { typeof(string), typeof(StringComparison) });
        internal static readonly MethodInfo AnyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(t => t.Name == "Any" && t.GetParameters().Count() == 2);
        internal static readonly MethodInfo AllMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public).First(t => t.Name == "All" && t.GetParameters().Count() == 2);
    }
}
