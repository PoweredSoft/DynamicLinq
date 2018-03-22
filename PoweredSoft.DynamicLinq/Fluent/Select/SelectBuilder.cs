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
    }

    public class SelectBuilder : IQueryBuilder
    {
        public List<SelectPart> Parts = new List<SelectPart>();
        public Type DestinationType { get; set; }
        public bool Empty => Parts?.Count == 0;
        public IQueryable Query { get; protected set; }

        public SelectBuilder(IQueryable query)
        {
            Query = query;
        }

        protected void throwIfUsedOrEmpty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException($"{propertyName} cannot end up be empty.");

            if (Parts.Any(t => t.PropertyName == propertyName))
                throw new Exception($"{propertyName} is already used");
        }

        public SelectBuilder Key(string propertyName, string path = null)
        {
            if (propertyName == null)
                propertyName = path.Split('.').LastOrDefault();

            throwIfUsedOrEmpty(propertyName);

            Parts.Add(new SelectPart
            {
                Path = path == null ? "Key" : $"Key.{path}",
                PropertyName = propertyName,
                SelectType = SelectTypes.Key
            });

            return this;
        }

        public SelectBuilder Path(string path, string propertyName = null)
        {
            if (propertyName == null)
                propertyName = path.Split('.').LastOrDefault();

            throwIfUsedOrEmpty(propertyName);

            Parts.Add(new SelectPart
            {
                Path = path, 
                PropertyName = propertyName,
                SelectType = SelectTypes.Path
            });

            return this;
        }

        public SelectBuilder Count(string propertyName)
        {
            throwIfUsedOrEmpty(propertyName);
            Parts.Add(new SelectPart
            {
                PropertyName = propertyName,
                SelectType = SelectTypes.Count
            });
            return this;
        }

        public SelectBuilder LongCount(string propertyName)
        {
            throwIfUsedOrEmpty(propertyName);
            Parts.Add(new SelectPart
            {
                PropertyName = propertyName,
                SelectType = SelectTypes.LongCount
            });
            return this;
        }

        public SelectBuilder Sum(string path, string propertyName = null)
        {
            if (propertyName == null)
                propertyName = path.Split('.').LastOrDefault();

            throwIfUsedOrEmpty(propertyName);

            Parts.Add(new SelectPart
            {
                Path = path,
                PropertyName = propertyName,
                SelectType = SelectTypes.Sum
            });
            return this;
        }

        public SelectBuilder Average(string path, string propertyName = null)
        {
            if (propertyName == null)
                propertyName = path.Split('.').LastOrDefault();

            throwIfUsedOrEmpty(propertyName);

            Parts.Add(new SelectPart
            {
                Path = path,
                PropertyName = propertyName,
                SelectType = SelectTypes.Average
            });
            return this;
        }

        public SelectBuilder ToList(string propertyName)
        {
            throwIfUsedOrEmpty(propertyName);
            Parts.Add(new SelectPart
            {
                PropertyName = propertyName,
                SelectType = SelectTypes.ToList
            });
            return this;
        }

        public SelectBuilder PathToList(string path, string propertyName = null)
        {
            if (propertyName == null)
                propertyName = path.Split('.').LastOrDefault();

            throwIfUsedOrEmpty(propertyName);

            Parts.Add(new SelectPart
            {
                Path = path,
                PropertyName = propertyName,
                SelectType = SelectTypes.PathToList
            });

            return this;
        }

        public virtual IQueryable Build()
        {
            if (Empty)
                throw new Exception("No select specified, please specify at least one select path");

            var partsTuple = Parts.Select(t => (selectType: t.SelectType, propertyName: t.PropertyName, path: t.Path)).ToList();
            return QueryableHelpers.Select(Query, partsTuple, DestinationType);
        }
    }
}
