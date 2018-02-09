using System;
using System.Collections.Generic;
using Lucene.Net.Linq.Fluent;
using Lucene.Net.Linq.Mapping;
using Lucene.Net.Util;
using NUnit.Framework;

namespace Lucene.Net.Linq.Tests.Fluent
{
    public class FluentDocumentMapperTestBase
    {
        protected SampleMap map;

        public class Sample
        {
            public string Name { get; set; }

            public int Id { get; set; }

            public float Score { get; set; }

            public DateTimeOffset Date { get; set; }

            public IEnumerable<int> Numbers { get; set; }

            public IEnumerable<Uri> Urls { get; set; }

            public float Boost { get; set; }
        }

        public class SampleMap : ClassMap<Sample>
        {
            public SampleMap()
                :base(LuceneVersion.LUCENE_48)
            {
            }
        }

        [SetUp]
        public void SetUp()
        {
            map = new SampleMap();
        }

        protected ReflectionFieldMapper<Sample> GetMappingInfo(string propertyName)
        {
            return GetMappingInfo<ReflectionFieldMapper<Sample>>(propertyName);
        }

        protected TMapper GetMappingInfo<TMapper>(string propertyName)
        {
            return (TMapper)map.ToDocumentMapper().GetMappingInfo(propertyName);
        }
    }
}
