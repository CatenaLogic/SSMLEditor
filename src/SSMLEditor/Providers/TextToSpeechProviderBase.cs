namespace SSMLEditor.Providers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;

    public abstract class TextToSpeechProviderBase : ITextToSpeechProvider
    {
        protected TextToSpeechProviderBase()
        {
            Properties = new List<TtsProperty>();
        }

        public abstract string Name { get; }
        public List<TtsProperty> Properties { get; private set; }

        public TtsProperty this[string propertyName]
        {
            get
            {
                return Properties.FirstOrDefault(x => x.Name.EqualsIgnoreCase(propertyName));
            }
        }

        public abstract Task<Stream> ExecuteAsync(string ssml);
        public abstract Task<IEnumerable<TtsLanguage>> GetLanguagesAsync();
        public abstract Task<IEnumerable<TtsVoice>> GetVoicesAsync(TtsLanguage language);
    }
}
