using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Queries;
using Lucene.Net.Search;
//using Lucene.Net.Search.Function;

namespace Lucene.Net.Linq.Search.Function
{
    internal class DelegatingCustomScoreQuery<T> : CustomScoreQuery
    {
        private readonly Func<Document, T> convertFunction;
        private readonly Func<T, float> scoreFunction;

        public DelegatingCustomScoreQuery(Query subQuery, Func<Document, T> convertFunction, Func<T, float> scoreFunction)
            : base(subQuery)
        {
            this.convertFunction = convertFunction;
            this.scoreFunction = scoreFunction;
        }

        protected override CustomScoreProvider GetCustomScoreProvider(AtomicReaderContext context)
        {
            return new DelegatingScoreProvider(context, convertFunction, scoreFunction);
        }

        class DelegatingScoreProvider : CustomScoreProvider
        {
            private readonly Func<Document, T> convertFunction;
            private readonly Func<T, float> scoreFunction;

            public DelegatingScoreProvider(AtomicReaderContext context, Func<Document, T> convertFunction, Func<T, float> scoreFunction)
                : base(context)
            {
                this.convertFunction = convertFunction;
                this.scoreFunction = scoreFunction;
            }

            public override float CustomScore(int doc, float subQueryScore, float valSrcScore)
            {
                var val = base.CustomScore(doc, subQueryScore, valSrcScore);

                return val * scoreFunction(convertFunction(m_context.AtomicReader.Document(doc)));
            }
        }
    }
}
