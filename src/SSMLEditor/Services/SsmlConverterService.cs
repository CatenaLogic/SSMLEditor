namespace SSMLEditor.Services
{
    using System;
    using System.Text;
    using System.Windows.Documents;

    public class SsmlConverterService : ISsmlConverterService
    {
        public string ConvertToSsml(FlowDocument flowDocument)
        {
            ArgumentNullException.ThrowIfNull(flowDocument);

            var stringBuilder = new StringBuilder();


            var ssml = stringBuilder.ToString();
            return ssml;
        }

        public FlowDocument ConvertToFlowDocument(string ssml)
        {
            var flowDocument = new FlowDocument();



            return flowDocument;
        }
    }
}
