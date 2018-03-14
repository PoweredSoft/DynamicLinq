using System;
using System.Collections.Generic;
using System.Text;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public abstract partial class WhereBuilderBase
    {
        public WhereBuilderBase And(string path, ConditionOperators conditionOperator, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Compare(path, conditionOperator, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, and: true, stringComparision: stringComparision);

        public WhereBuilderBase Or(string path, ConditionOperators conditionOperator, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Compare(path, conditionOperator, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, and: false, stringComparision: stringComparision);

        public WhereBuilderBase And(Action<WhereBuilderBase> subQuery)
            => SubQuery(subQuery, true);

        public WhereBuilderBase Or(Action<WhereBuilderBase> subQuery)
            => SubQuery(subQuery, false);

        #region equal
        public WhereBuilderBase Equal(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.Equal, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase AndEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.Equal, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase OrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Or(path, ConditionOperators.Equal, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);
        #endregion

        #region not equal
        public WhereBuilderBase NotEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.NotEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase AndNotEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.NotEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase OrNotEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Or(path, ConditionOperators.NotEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);
        #endregion

        #region GreaterThan
        public WhereBuilderBase GreaterThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.GreaterThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilderBase AndGreaterThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.GreaterThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilderBase OrGreaterThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Or(path, ConditionOperators.GreaterThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);
        #endregion

        #region GreaterThanOrEqual
        public WhereBuilderBase GreaterThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.GreaterThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilderBase AndGreaterThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.GreaterThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilderBase OrGreaterThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Or(path, ConditionOperators.GreaterThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);
        #endregion

        #region LessThan
        public WhereBuilderBase LessThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.LessThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilderBase AndLessThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.LessThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilderBase OrLessThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Or(path, ConditionOperators.LessThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);
        #endregion

        #region LessThanOrEqual
        public WhereBuilderBase LessThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.LessThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilderBase AndLessThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.LessThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilderBase OrLessThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Or(path, ConditionOperators.LessThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);
        #endregion

        #region contains
        public WhereBuilderBase Contains(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.Contains, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase AndContains(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.Contains, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase OrContains(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Or(path, ConditionOperators.Contains, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);
        #endregion

        #region starts with
        public WhereBuilderBase StartsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.StartsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase AndStartsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.StartsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase OrStartsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Or(path, ConditionOperators.StartsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);
        #endregion

        #region ends with
        public WhereBuilderBase EndsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.EndsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase AndEndsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.EndsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase OrEndsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Or(path, ConditionOperators.EndsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);
        #endregion

        #region In
        public WhereBuilderBase In(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.In, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase AndIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.In, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase OrIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Or(path, ConditionOperators.In, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);
        #endregion

        #region NotIn
        public WhereBuilderBase NotIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.NotIn, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase AndNotIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.NotIn, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilderBase OrNotIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Or(path, ConditionOperators.NotIn, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);
        #endregion
    }
}
