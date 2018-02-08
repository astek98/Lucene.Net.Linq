using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Util;

namespace Lucene.Net.Linq.Analysis
{
    /// <summary>
    /// Decorates <see cref="KeywordAnalyzer"/> to convert the token stream
    /// to lowercase, allowing queries with different case-spelling to match.
    /// </summary>
    public class CaseInsensitiveKeywordAnalyzer : Analyzer
    {
        protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            return new TokenStreamComponents(new LowerCaseTokenizer(LuceneVersion.LUCENE_48, reader));
        }
    }
}
