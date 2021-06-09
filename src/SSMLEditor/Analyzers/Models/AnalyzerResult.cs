namespace SSMLEditor.Analyzers
{
    public class AnalyzerResult
    {
        public int StartIndex { get; set; }

        public int Length { get; set; }

        public int EndIndex
        {
            get { return StartIndex + Length; }
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public AnalyzerResultType ResultType { get; set; }
    }
}
