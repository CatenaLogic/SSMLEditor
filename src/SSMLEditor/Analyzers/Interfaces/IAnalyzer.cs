namespace SSMLEditor.Analyzers
{
    using System.Collections.Generic;

    public interface IAnalyzer
    {
        IAsyncEnumerable<AnalyzerResult> AnalyzeAsync(AnalyzerContext context);
    }
}
