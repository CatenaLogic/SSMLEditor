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

        public string Name { get; set; }

        public List<TtsProperty> Properties { get; private set; }

        public TtsProperty this[string propertyName]
        {
            get
            {
                return Properties.LastOrDefault(x => x.Name.EqualsIgnoreCase(propertyName));
            }
        }

        public abstract Task<Stream> ExecuteAsync(string ssml);
        public abstract Task<IEnumerable<TtsLanguage>> GetLanguagesAsync();
        public abstract Task<IEnumerable<TtsVoice>> GetVoicesAsync(TtsLanguage language);

        public virtual void RemoveDuplicateProperties()
        {
            for (var i = 0; i < Properties.Count; i++)
            {
                var property = Properties[i];
                
                for (var j = i + 1; j < Properties.Count; j++)
                {
                    var otherProperty = Properties[j];

                    if (property.Name == otherProperty.Name)
                    {
                        // Remove first item
                        Properties.RemoveAt(i--);
                        break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
