using System;
using System.Reflection;
using Lucene.Net.Documents;
using Lucene.Net.Linq.Search;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Lucene.Net.Linq.Util
{
    internal static class NumericRangeUtils
    {
        internal static Query CreateNumericRangeQuery(string fieldName, ValueType lowerBound, ValueType upperBound, RangeType lowerRange, RangeType upperRange)
        {
            if (lowerBound == null && upperBound == null)
            {
                throw new ArgumentException("lowerBound and upperBound may not both be null.");
            }

            if (lowerBound == null)
            {
                lowerBound = (ValueType) upperBound.GetType().GetField("MinValue").GetValue(null);
            }
            else if (upperBound == null)
            {
                upperBound = (ValueType) lowerBound.GetType().GetField("MaxValue").GetValue(null);
            }

            if (lowerBound.GetType() != upperBound.GetType())
            {
                throw new ArgumentException("Cannot compare different value types " + lowerBound.GetType() + " and " + upperBound.GetType());
            }

            lowerBound = ToNumericFieldValue(lowerBound);
            upperBound = ToNumericFieldValue(upperBound);

            var minInclusive = lowerRange == RangeType.Inclusive;
            var maxInclusive = upperRange == RangeType.Inclusive;

            if (lowerBound is int)
            {
                return NumericRangeQuery.NewInt32Range(fieldName, (int)lowerBound, (int)upperBound, minInclusive, maxInclusive);
            }
            if (lowerBound is long)
            {
                return NumericRangeQuery.NewInt64Range(fieldName, (long)lowerBound, (long)upperBound, minInclusive, maxInclusive);
            }
            if (lowerBound is float)
            {
                return NumericRangeQuery.NewSingleRange(fieldName, (float)lowerBound, (float)upperBound, minInclusive, maxInclusive);
            }
            if (lowerBound is double)
            {
                return NumericRangeQuery.NewDoubleRange(fieldName, (double)lowerBound, (double)upperBound, minInclusive, maxInclusive);
            }

            throw new NotSupportedException("Unsupported numeric range type " + lowerBound.GetType());
        }

        /// <summary>
        /// Converts supported value types such as DateTime to an underlying ValueType that is supported by
        /// <c ref="NumericRangeQuery"/>.
        /// </summary>
        [Obsolete]
        internal static ValueType ToNumericFieldValue(this ValueType value)
        {
            // TODO: replace with converters
            if (value is DateTime)
            {
                return ((DateTime)value).ToUniversalTime().Ticks;
            }
            if (value is DateTimeOffset)
            {
                return ((DateTimeOffset)value).Ticks;
            }

            return value;
        }

        internal static BytesRef ToPrefixCoded(this ValueType value)
        {
            BytesRef result = new BytesRef();
            if (value is int)
            {
                NumericUtils.Int32ToPrefixCoded((int)value, 0, result);
                return result;
            }
            if (value is long)
            {
                NumericUtils.Int64ToPrefixCoded((long)value, 0, result);
                return result;
            }
            if (value is double)
            {
                var sl = NumericUtils.DoubleToSortableInt64((double)value);
                NumericUtils.Int64ToPrefixCoded(sl, 0, result);
                return result;
            }
            if (value is float)
            {
                var si = NumericUtils.SingleToSortableInt32((float)value);
                NumericUtils.Int64ToPrefixCoded(si, 0, result);
                return result;
            }

            throw new NotSupportedException("ValueType " + value.GetType() + " not supported.");
        }

        internal static NumericDocValuesField SetValue(this NumericDocValuesField field, ValueType value)
        {
            if (value.GetType().IsEnum)
            {
                value = (ValueType) Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
            }

            if (value is int)
            {
                field.SetInt32Value((int) value);
                return field;
            }
            if (value is long)
            {
                field.SetInt64Value((long)value);
                return field;
            }
            if (value is double)
            {
                field.SetDoubleValue((double) value);
                return field;
            }
            if (value is float)
            {
                field.SetSingleValue((float) value);
                return field;
            }

            throw new ArgumentException("Unable to store ValueType " + value.GetType() + " as NumericField.", "value");
        }

        /// <summary>
        /// See https://issues.apache.org/jira/browse/LUCENENET-519.
        /// <see cref="NumericField"/> uses <see cref="Field.Index.ANALYZED_NO_NORMS"/> and does
        /// not allow alternative indexing methods to be used. This prevents boost from being applied
        /// when a document is being indexed.
        /// </summary>
        internal static NumericDocValuesField ForceDisableOmitNorms(this NumericDocValuesField field)
        {
            const string fieldName = "internalOmitNorms";
            var fieldInfo = typeof(Field).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (fieldInfo == null)
            {
                throw new InvalidOperationException(string.Format("Type {0} does not have a non-public field named {1}.", typeof(Field), fieldName));
            }

            fieldInfo.SetValue(field, false);

            return field;
        }
    }

    internal static class TypeExtensions
    {
        internal static SortFieldType ToSortField(this Type valueType)
        {
            if (valueType == typeof(long))
            {
                return SortFieldType.INT64;
            }
            if (valueType == typeof(int))
            {
                return SortFieldType.INT32;
            }
            if (valueType == typeof(double))
            {
                return SortFieldType.DOUBLE;
            }
            if (valueType == typeof(float))
            {
                return SortFieldType.SINGLE;
            }

            return SortFieldType.CUSTOM;
        }

        internal static Type GetUnderlyingType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

    }
}
