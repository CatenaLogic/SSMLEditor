namespace SSMLEditor.Analyzers;

public class AnalyzerResult
{
    public int StartIndex { get; set; }

    public int Length { get; set; }

    public int EndIndex
    {
        get { return StartIndex + Length; }
    }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public AnalyzerResultType ResultType { get; set; }
}
