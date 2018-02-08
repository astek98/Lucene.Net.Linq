using System;
using System.ComponentModel;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;
using Lucene.Net.Util.Fst;

namespace Lucene.Net.Linq.Search
{
    internal class GenericConvertableFieldComparatorSource : FieldComparerSource
    {
        private readonly Type type;
        private readonly TypeConverter converter;

        public GenericConvertableFieldComparatorSource(Type type, TypeConverter converter)
        {
            this.type = type;
            this.converter = converter;
        }

        public override FieldComparer NewComparer(string fieldname, int numHits, int sortPos, bool reversed)
        {
            var genericType = typeof(GenericConvertableFieldComparator<>).MakeGenericType(type);
            var ctr = genericType.GetConstructor(new[] { typeof(int), typeof(string), typeof(TypeConverter) });
            return (FieldComparer)ctr.Invoke(new object[] { numHits, fieldname, converter });
        }

        public class GenericConvertableFieldComparator<TComparable> : GenericFieldComparator<TComparable> where TComparable : IComparable<TComparable>
        {
            private readonly TypeConverter converter;

            public GenericConvertableFieldComparator(int numHits, string field, TypeConverter converter)
                : base(numHits, field)
            {
                this.converter = converter;
            }

            protected override TComparable[] GetCurrentReaderValues(IndexReader reader, int docBase)
            {
                BytesRef result = new BytesRef();
                FieldCache.DEFAULT.GetTerms((AtomicReader) reader, field, true).Get(docBase, result);
                var str = result.Utf8ToString();
                return str.Select(s => s == null ? default(TComparable) : converter.ConvertFrom(s)).Cast<TComparable>().ToArray();
            }

            public override int CompareValues(object first, object second)
            {
                throw new NotImplementedException();
            }

            public override void SetTopValue(object value)
            {
                throw new NotImplementedException();
            }

            public override int CompareTop(int doc)
            {
                throw new NotImplementedException();
            }

            public override FieldComparer SetNextReader(AtomicReaderContext context)
            {
                throw new NotImplementedException();
            }
        }


    }
}
