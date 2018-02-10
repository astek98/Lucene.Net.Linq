using System;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Index;
using Lucene.Net.Linq.Abstractions;
using Lucene.Net.Linq.Analysis;
using Lucene.Net.Linq.Mapping;
using Lucene.Net.Store;
using Lucene.Net.Util;
using NUnit.Framework;
using Rhino.Mocks;


namespace Lucene.Net.Linq.Tests
{
    [TestFixture]
    public class LuceneDataProviderTests
    {
        public class Item
        {
            public int Id { get; set; }
        }

        [Test]
        public void OpenSessionWithoutWriterCreatesIndexWhenMissing()
        {
            var provider = new LuceneDataProvider(new RAMDirectory(), new SimpleAnalyzer(LuceneVersion.LUCENE_48), LuceneVersion.LUCENE_48);

            TestDelegate call = () => provider.OpenSession<Item>();

            Assert.That(call, Throws.Nothing);
        }

        [Test]
        public void OpenSessionThrowsWhenDocumentMapperDoesNotImplementModificationDetector()
        {
            var provider = new LuceneDataProvider(new RAMDirectory(), new SimpleAnalyzer(LuceneVersion.LUCENE_48),LuceneVersion.LUCENE_48);

            TestDelegate call = () => provider.OpenSession(MockRepository.GenerateStrictMock<IDocumentMapper<Item>>());

            Assert.That(call, Throws.ArgumentException.With.Property("ParamName").EqualTo("documentMapper"));
        }

        [Test]
        public void RegisterCacheWarmingCallback()
        {
            var directory = new RAMDirectory();
            var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, new CaseInsensitiveKeywordAnalyzer())
            {
                OpenMode = OpenMode.CREATE_OR_APPEND,
            };
            var writer = new IndexWriter(directory, config);
            var provider = new LuceneDataProvider(directory, new SimpleAnalyzer(LuceneVersion.LUCENE_48), LuceneVersion.LUCENE_48, writer);

            var count = -1;

            provider.RegisterCacheWarmingCallback<Item>(q => count = q.Count());

            provider.Context.Reload();

            Assert.That(count, Is.EqualTo(0));
        }

        [Test]
        public void CreatesIndex()
        {
            var provider = new LuceneDataProvider(new RAMDirectory(),LuceneVersion.LUCENE_48);

            Assert.That(provider.AsQueryable<A>().Count(), Is.EqualTo(0));
        }

        [Test]
        public void DisposesInternallyCreatedWriter()
        {
            var provider = new TestableLuceneDataProvider(new RAMDirectory(), LuceneVersion.LUCENE_48);

            provider.Dispose();

            provider.IndexWriter.AssertWasCalled(w => w.Dispose());
        }

        [Test]
        public void UsesSameWriterInstance()
        {
            var provider = new TestableLuceneDataProvider(new RAMDirectory(), LuceneVersion.LUCENE_48);

            Assert.That(provider.IndexWriter, Is.SameAs(provider.IndexWriter), "provider.IndexWriter");
        }

        [Test]
        public void CreatesNewWriterAfterRollback()
        {
            var provider = new TestableLuceneDataProvider(new RAMDirectory(), LuceneVersion.LUCENE_48);

            var first = provider.IndexWriter;

            first.Expect(iw => iw.IsClosed).Return(true);

            var next = provider.IndexWriter;

            Assert.That(next, Is.Not.SameAs(first), "Should create new writer when current is closed.");
        }

        [Test]
        public void ThrowsWhenExternallyCreatedWriterIsClosed()
        {
            var writer = MockRepository.GenerateStrictMock<IIndexWriter>();
            var provider = new LuceneDataProvider(new RAMDirectory(), LuceneVersion.LUCENE_48, writer, new object());

            writer.Expect(iw => iw.IsClosed).Return(true);

            TestDelegate call = () => provider.IndexWriter.ToString();

            Assert.That(call, Throws.InvalidOperationException);
        }

        [Test]
        public void DoesNotDisposeExternallyProvidesWriter()
        {
            var writer = MockRepository.GenerateMock<IIndexWriter>();
            var provider = new LuceneDataProvider(new RAMDirectory(), new KeywordAnalyzer(), LuceneVersion.LUCENE_48, writer, new object());

            provider.Dispose();

            writer.AssertWasNotCalled(w => w.Dispose());
        }

        public class TestableLuceneDataProvider : LuceneDataProvider
        {
            public TestableLuceneDataProvider(Directory directory, LuceneVersion version) : base(directory, version)
            {
            }

            protected override IIndexWriter GetIndexWriter(Analyzer analyzer)
            {
                return MockRepository.GenerateMock<IIndexWriter>();
            }
        }

        [Test]
        [Ignore("Fixed")]
        public void MergesAnalyzersForSessionsOfDifferentTypes()
        {
            var provider = new LuceneDataProvider(new RAMDirectory(), LuceneVersion.LUCENE_48);

            provider.OpenSession<A>();
            provider.OpenSession<B>();

//            Assert.That(provider.Analyzer["Prop1"], Is.InstanceOf<SimpleAnalyzer>());
//            Assert.That(provider.Analyzer["Prop2"], Is.InstanceOf<WhitespaceAnalyzer>());
        }

        public class A
        {
            [Field(Analyzer=typeof(SimpleAnalyzer))]
            public string Prop1 { get; set; }
        }

        public class B
        {
            [Field(Analyzer = typeof(WhitespaceAnalyzer))]
            public string Prop2 { get; set; }
        }
    }
}
