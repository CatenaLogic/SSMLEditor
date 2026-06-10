namespace SSMLEditor.Providers;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public interface ITextToSpeechProvider
{
    string Name { get; }

    TtsProperty this[string propertyName] { get; }

    List<TtsProperty> Properties { get; }

    Task<IReadOnlyList<TtsLanguage>> GetLanguagesAsync();

    Task<IReadOnlyList<TtsVoice>> GetVoicesAsync(TtsLanguage language);

    Task<Stream> ExecuteAsync(string ssml);

    void RemoveDuplicateProperties();
}
