namespace SSMLEditor.Analyzers
{
    using System.Threading;
    using System.Xml.Linq;

    public class AnalyzerContext
    {
        public AnalyzerContext(string documentAsText, XDocument document, CancellationToken cancellationToken)
        {
            DocumentAsText = documentAsText;
            DocumentLines = documentAsText.Split("\r\n");
            Document = document;
            CancellationToken = cancellationToken;
        }

        public string[] DocumentLines { get; }

        public string DocumentAsText { get; }

        public XDocument Document { get; }
        public CancellationToken CancellationToken { get; }
    }
}
