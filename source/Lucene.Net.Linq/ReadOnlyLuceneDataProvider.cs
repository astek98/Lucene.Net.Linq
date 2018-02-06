using System;
using Lucene.Net.Analysis;
using Lucene.Net.Linq.Abstractions;
using Lucene.Net.Linq.Mapping;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace Lucene.Net.Linq
{
    public class ReadOnlyLuceneDataProvider : LuceneDataProvider
    {
        public ReadOnlyLuceneDataProvider(Directory directory, LuceneVersion version) : base(directory, version)
        {
        }

        public ReadOnlyLuceneDataProvider(Directory directory, Analyzer externalAnalyzer, LuceneVersion version) : base(directory, externalAnalyzer, version)
        {
        }

        protected override IIndexWriter GetIndexWriter(Analyzer analyzer)
        {
            return null;
        }

        public override ISession<T> OpenSession<T>(ObjectLookup<T> factory, IDocumentMapper<T> documentMapper, IDocumentModificationDetector<T> documentModificationDetector)
        {
            throw new InvalidOperationException("Cannot open sessions in read-only mode.");
        }
    }
}
