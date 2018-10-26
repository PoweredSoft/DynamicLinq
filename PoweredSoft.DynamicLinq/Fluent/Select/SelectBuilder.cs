using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public class SelectPart
    {
        public string Path { get; set; }
        public string PropertyName { get; set; }
        public SelectTypes SelectType { get; set; }
        public SelectCollectionHandling SelectCollectionHandling { get; set; }
    }

    public class SelectBuilder : IQueryBuilder
    {
        public List<SelectPart> Parts = new List<SelectPart>();
        public Type DestinationType { get; set; }
        public bool Empty => Parts?.Count == 0;
        public IQueryable Query { get; protected set; }
        public bool IsNullCheckingEnabled { get; protected set; }

        public SelectBuilder(IQueryable query)
        {
            Query = query;
        }

        public SelectBuilder NullChecking(bool check = true)
        {
            IsNullCheckingEnabled = check;
            return this;
        }

        protected void ThrowIfUsedOrEmpty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException($"{propertyName} cannot end up be empty.");

            if (Parts.Any(t => t.PropertyName == propertyName))
                throw new Exception($"{propertyName} is already used");
        }

        public SelectBuilder Aggregate(string path, SelectTypes type, string propertyName = null, SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs)
        {
            if (propertyName == null && path == null)
                throw new Exception("if property name is not specified, a path must be supplied.");

            if (propertyName == null)
                propertyName = path.Split('.').LastOrDefault();

            ThrowIfUsedOrEmpty(propertyName);

            Parts.Add(new SelectPart
            {
                Path = path,
                PropertyName = propertyName,
                SelectType = type,
                SelectCollectionHandling = selectCollectionHandling
            });

            return this;
        }

        public SelectBuilder Key(string propertyName, string path = null) => Aggregate(path == null ? "Key" : $"Key.{path}", SelectTypes.Key, propertyName);
        public SelectBuilder Path(string path, string propertyName = null) => Aggregate(path, SelectTypes.Path, propertyName);
        public SelectBuilder Count(string propertyName) => Aggregate(null, SelectTypes.Count, propertyName);
        public SelectBuilder LongCount(string propertyName) => Aggregate(null, SelectTypes.LongCount, propertyName);
        public SelectBuilder Sum(string path, string propertyName = null) => Aggregate(path, SelectTypes.Sum, propertyName);
        public SelectBuilder Average(string path, string propertyName = null) => Aggregate(path, SelectTypes.Average, propertyName);
        public SelectBuilder Min(string path, string propertyName = null) => Aggregate(path, SelectTypes.Min, propertyName);
        public SelectBuilder Max(string path, string propertyName = null) => Aggregate(path, SelectTypes.Max, propertyName);
        public SelectBuilder ToList(string propertyName) => Aggregate(null, SelectTypes.ToList, propertyName);
        public SelectBuilder LastOrDefault(string propertyName) => Aggregate(null, SelectTypes.LastOrDefault, propertyName);
        public SelectBuilder FirstOrDefault(string propertyName) => Aggregate(null, SelectTypes.FirstOrDefault, propertyName);
        public SelectBuilder Last(string propertyName) => Aggregate(null, SelectTypes.Last, propertyName);
        public SelectBuilder First(string propertyName) => Aggregate(null, SelectTypes.First, propertyName);

        [System.Obsolete("Use ToList instead")]
        public SelectBuilder PathToList(string path, string propertyName = null, SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs)
                => ToList(path, propertyName, selectCollectionHandling);

        public SelectBuilder ToList(string path, string propertyName = null, SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs)
            => Aggregate(path, SelectTypes.ToList, propertyName: propertyName, selectCollectionHandling: selectCollectionHandling);

        public SelectBuilder First(string path, string propertyName = null, SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs)
            => Aggregate(path, SelectTypes.First, propertyName: propertyName, selectCollectionHandling: selectCollectionHandling);

        public SelectBuilder FirstOrDefault(string path, string propertyName = null, SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs)
            => Aggregate(path, SelectTypes.FirstOrDefault, propertyName: propertyName, selectCollectionHandling: selectCollectionHandling);

        public SelectBuilder Last(string path, string propertyName = null, SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs)
            => Aggregate(path, SelectTypes.Last, propertyName: propertyName, selectCollectionHandling: selectCollectionHandling);

        public SelectBuilder LastOrDefault(string path, string propertyName = null, SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs)
            => Aggregate(path, SelectTypes.LastOrDefault, propertyName: propertyName, selectCollectionHandling: selectCollectionHandling);

        public virtual IQueryable Build()
        {
            if (Empty)
                throw new Exception("No select specified, please specify at least one select path");

            var partsTuple = Parts.Select(t => (selectType: t.SelectType, propertyName: t.PropertyName, path: t.Path, selectCollectionHandling: t.SelectCollectionHandling)).ToList();
            return QueryableHelpers.Select(Query, partsTuple, DestinationType, nullChecking: IsNullCheckingEnabled);
        }
    }
}
