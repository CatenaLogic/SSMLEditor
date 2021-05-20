namespace SSMLEditor.Providers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface ITextToSpeechProvider
    {
        string Name { get; }

        IEnumerable<Voice> GetVoices();

        Task<Stream> ExecuteAsync(string ssml);
    }
}
