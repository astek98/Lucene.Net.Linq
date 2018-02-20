using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Lucene.Net.Linq.Search
{
    public abstract class FieldComparator<T> : FieldComparer<T>
    {
        protected string field;
        protected T[] values;
        protected T[] currentReaderValues;
        protected T bottom;

        protected FieldComparator(int numHits, string field)
        {
            this.field = field;
            this.values = new T[numHits];
        }

        public override void Copy(int slot, int doc)
        {
            values[slot] = currentReaderValues[doc];
        }

        public override void SetBottom(int bottom)
        {
            this.bottom = values[bottom];
        }

        public override FieldComparer SetNextReader(AtomicReaderContext context)
        {
            currentReaderValues = GetCurrentReaderValues(context.Reader, context.DocBase);
            return this;
        }

        protected abstract T[] GetCurrentReaderValues(IndexReader reader, int docBase);
    }
}
