namespace SSMLEditor.Services
{
    using System.Text;
    using System.Windows.Documents;
    using Catel;

    public class SsmlConverterService : ISsmlConverterService
    {
        public string ConvertToSsml(FlowDocument flowDocument)
        {
            Argument.IsNotNull(() => flowDocument);

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
