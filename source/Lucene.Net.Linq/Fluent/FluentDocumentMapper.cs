using Lucene.Net.Linq.Mapping;
using Lucene.Net.Util;

namespace Lucene.Net.Linq.Fluent
{
    internal class FluentDocumentMapper<T> : DocumentMapperBase<T>
    {
        public FluentDocumentMapper(LuceneVersion version) : base(version)
        {
        }
    }
}
