using System;
using System.Collections.Generic;
using System.Text;

namespace PoweredSoft.DynamicLinq.Fluent
{
    public partial class WhereBuilder
    {
        public WhereBuilder And(string path, ConditionOperators conditionOperator, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => Compare(path, conditionOperator, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, and: true, stringComparision: stringComparision, negate: negate);

        public WhereBuilder Or(string path, ConditionOperators conditionOperator, object value,
            QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => Compare(path, conditionOperator, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, and: false, stringComparision: stringComparision, negate: negate);

        public WhereBuilder And(Action<WhereBuilder> subQuery)
            => SubQuery(subQuery, true);

        public WhereBuilder Or(Action<WhereBuilder> subQuery)
            => SubQuery(subQuery, false);

        #region equal
        public WhereBuilder Equal(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.Equal, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder AndEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.Equal, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder OrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => Or(path, ConditionOperators.Equal, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);
        #endregion

        #region not equal
        public WhereBuilder NotEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.NotEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder AndNotEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.NotEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder OrNotEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => Or(path, ConditionOperators.NotEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);
        #endregion

        #region GreaterThan
        public WhereBuilder GreaterThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.GreaterThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilder AndGreaterThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.GreaterThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilder OrGreaterThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Or(path, ConditionOperators.GreaterThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);
        #endregion

        #region GreaterThanOrEqual
        public WhereBuilder GreaterThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.GreaterThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilder AndGreaterThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.GreaterThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilder OrGreaterThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Or(path, ConditionOperators.GreaterThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);
        #endregion

        #region LessThan
        public WhereBuilder LessThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.LessThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilder AndLessThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.LessThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilder OrLessThan(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Or(path, ConditionOperators.LessThan, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);
        #endregion

        #region LessThanOrEqual
        public WhereBuilder LessThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.LessThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilder AndLessThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => And(path, ConditionOperators.LessThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);

        public WhereBuilder OrLessThanOrEqual(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any)
            => Or(path, ConditionOperators.LessThanOrEqual, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling);
        #endregion

        #region contains
        public WhereBuilder Contains(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.Contains, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder AndContains(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.Contains, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder OrContains(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => Or(path, ConditionOperators.Contains, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);
        #endregion

        #region not contains
        public WhereBuilder NotContains(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.NotContains, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder AndNotContains(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.NotContains, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder OrNotContains(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => Or(path, ConditionOperators.NotContains, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);
        #endregion

        #region starts with
        public WhereBuilder StartsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.StartsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder AndStartsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.StartsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder OrStartsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => Or(path, ConditionOperators.StartsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);
        #endregion

        #region ends with
        public WhereBuilder EndsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.EndsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder AndEndsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => And(path, ConditionOperators.EndsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);

        public WhereBuilder OrEndsWith(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null, bool negate = false)
            => Or(path, ConditionOperators.EndsWith, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision, negate: negate);
        #endregion

        #region In
        public WhereBuilder In(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.In, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilder AndIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.In, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilder OrIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Or(path, ConditionOperators.In, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);
        #endregion

        #region NotIn
        public WhereBuilder NotIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.NotIn, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilder AndNotIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => And(path, ConditionOperators.NotIn, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);

        public WhereBuilder OrNotIn(string path, object value, QueryConvertStrategy convertStrategy = QueryConvertStrategy.ConvertConstantToComparedPropertyOrField, QueryCollectionHandling collectionHandling = QueryCollectionHandling.Any, StringComparison? stringComparision = null)
            => Or(path, ConditionOperators.NotIn, value, convertStrategy: convertStrategy, collectionHandling: collectionHandling, stringComparision: stringComparision);
        #endregion
    }
}
