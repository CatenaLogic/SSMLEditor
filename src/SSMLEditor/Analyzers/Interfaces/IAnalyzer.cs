namespace SSMLEditor.Analyzers
{
    using System.Collections.Generic;
    using System.Xml.Linq;

    public interface IAnalyzer
    {
        IAsyncEnumerable<AnalyzerResult> AnalyzeAsync(AnalyzerContext context);
    }
}
