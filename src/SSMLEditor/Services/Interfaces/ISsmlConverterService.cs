namespace SSMLEditor.Services
{
    using System.Windows.Documents;

    public interface ISsmlConverterService
    {
        FlowDocument ConvertToFlowDocument(string ssml);
        string ConvertToSsml(FlowDocument flowDocument);
    }
}
