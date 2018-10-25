using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using PoweredSoft.DynamicLinq.Helpers;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class GroupBuilder : IQueryBuilder
    {
        public List<(string path, string propertyName)> Parts { get; set; } = new List<(string path, string propertyName)>();
        public Type Type { get; set; }
        public bool Empty => !Parts.Any();
        public Type EqualityComparerType { get; set; }

        public IQueryable Query { get; protected set; }

        public bool IsNullCheckingEnabled { get; protected set; } = false;

        public GroupBuilder(IQueryable query)
        {
            Query = query;
        }

        public GroupBuilder Path(string path, string propertyName = null)
        {
            if (propertyName == null)
            {
                var name = path;
                if (name.Contains("."))
                {
                    var parts = name.Split('.');
                    name = parts[parts.Length - 1]; // the last one.
                }

                if (Parts.Any(t => t.propertyName == name))
                    throw new Exception($"{name} is already taken by another group part, you can specify a property name instead to resolve this issue");

                propertyName = name;
            }

            Parts.Add((path, propertyName));
            return this;
        }

        public GroupBuilder NullChecking(bool nullChecking = true)
        {
            IsNullCheckingEnabled = nullChecking;
            return this;
        }

        public GroupBuilder UseType(Type type)
        {
            Type = type;
            return this;
        }

        public GroupBuilder EqualityComparer(Type type)
        {
            EqualityComparerType = type;
            return this;
        }

        public virtual IQueryable Build()
        {
            if (Empty)
                throw new Exception("No group specified, please specify at least one group");

            var ret = QueryableHelpers.GroupBy(Query, Query.ElementType, Parts, groupToType: Type, equalityCompareType: EqualityComparerType, nullChecking: IsNullCheckingEnabled);
            return ret;
        }
    }
}
