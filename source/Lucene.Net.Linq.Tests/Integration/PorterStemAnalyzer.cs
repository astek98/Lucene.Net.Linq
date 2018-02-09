using System;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace Lucene.Net.Linq.Tests.Integration
{
    public class PorterStemAnalyzer : Analyzer
    {
        public PorterStemAnalyzer()
        {
        }

        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
