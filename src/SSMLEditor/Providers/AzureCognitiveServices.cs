namespace SSMLEditor.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public class AzureCognitiveServices : ITextToSpeechProvider
    {
        public string Name => "Azure Cognitive Services";

        public IEnumerable<Voice> GetVoices()
        {
            var voices = new List<Voice>();

            // TODO: Add voices

            return voices;
        }

        public Task<Stream> ExecuteAsync(string ssml)
        {
            throw new NotImplementedException();
        }
    }
}
