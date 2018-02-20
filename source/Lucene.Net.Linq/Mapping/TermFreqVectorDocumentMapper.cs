using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Util;

namespace Lucene.Net.Linq.Mapping
{
    /// <summary>
    /// Extends <see cref="ReflectionDocumentMapper{T}"/> to collect
    /// <see cref="ITermFreqVector"/>s for each field that has term
    /// vector analysis enabled at index time. Term Vector analysis
    /// can be enabled by setting <see cref="FieldAttribute.TermVector"/>.
    /// </summary>
    public class TermFreqVectorDocumentMapper<T> : ReflectionDocumentMapper<T>
    {
        private readonly IDictionary<T, Term[]> map = new Dictionary<T, Term[]>();

        public TermFreqVectorDocumentMapper(LuceneVersion version) : base(version)
        {
        }

        public TermFreqVectorDocumentMapper(LuceneVersion version, Analyzer externalAnalyzer)
            : base(version, externalAnalyzer)
        {
        }

        public override void ToObject(Documents.Document source, IQueryExecutionContext context, T target)
        {
            base.ToObject(source, context, target);

            // TODO-MIG
            throw new NotImplementedException("Migration to LCNT 4.0.8");
            //map[target] = context.Searcher.IndexReader.GetTermVector(context.CurrentScoreDoc.Doc, String.Empty );
        }

        public Term[] this[T index]
        {
            get { return map[index]; }
        }
    }
}
