namespace SSMLEditor.Services
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Xml.Linq;
    using MethodTimer;
    using SSMLEditor.Analyzers;

    public class AnalyzerService : InterfaceFinderServiceBase<IAnalyzer>, IAnalyzerService
    {
        private readonly List<IAnalyzer> _analyzers = new List<IAnalyzer>();

        public AnalyzerService()
        {
            _analyzers.AddRange(GetAvailableItems());
        }

        [Time]
        public async IAsyncEnumerable<AnalyzerResult> AnalyzeAsync(string document, [EnumeratorCancellation]CancellationToken cancellationToken)
        {
            var failed = false;
            XDocument xmlDocument = null;

            try
            {
                xmlDocument = XDocument.Parse(document, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
            }
            catch (Exception)
            {
                failed = true;
            }

            if (failed)
            {
                yield return new AnalyzerResult
                {
                    Name = "Not valid XML",
                    Description = "Please make sure to document is valid XML",
                    StartIndex = 0,
                    Length = document.Length
                };
            }

            var context = new AnalyzerContext(document, xmlDocument, cancellationToken);

            foreach (var analyzer in _analyzers)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                await foreach (var analyzerResult in analyzer.AnalyzeAsync(context))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        yield break;
                    }

                    yield return analyzerResult;
                }
            }
        }
    }
}
