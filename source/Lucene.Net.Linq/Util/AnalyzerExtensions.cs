using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;

namespace Lucene.Net.Linq.Util
{
    internal static class AnalyzerExtensions
    {
        internal static string Analyze(this Analyzer analyzer, string fieldName, string pattern)
        {
            return analyzer.GetTerms(fieldName, pattern).Single();
        }

        internal static IEnumerable<string> GetTerms(this Analyzer analyzer, string fieldName, string pattern)
        {
            TokenStream s;

            try
            {
                s = analyzer.GetTokenStream(fieldName, new StringReader(pattern));
            }
            catch (IOException)
            {
                s = analyzer.GetTokenStream(fieldName, new StringReader(pattern));
            }

            try
            {
                while (s.IncrementToken())
                {
                    if (!s.HasAttribute<ITermToBytesRefAttribute>()) continue;

                    var attr = s.GetAttribute<ITermToBytesRefAttribute>();
                    yield return attr.ToString();
                }
            }
            finally
            {
                s.Dispose();
            }
        }
    }
}
