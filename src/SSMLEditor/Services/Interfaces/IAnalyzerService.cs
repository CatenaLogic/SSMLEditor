namespace SSMLEditor.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using SSMLEditor.Analyzers;

    public interface IAnalyzerService
    {
        IAsyncEnumerable<AnalyzerResult> AnalyzeAsync(string document, CancellationToken cancellationToken);
    }
}
