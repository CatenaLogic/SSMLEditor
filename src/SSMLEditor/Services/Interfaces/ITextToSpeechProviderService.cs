namespace SSMLEditor.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SSMLEditor.Providers;

    public interface ITextToSpeechProviderService
    {
        List<ITextToSpeechProvider> Providers { get; }

        Task LoadAsync();

        Task SaveAsync();

        IEnumerable<ITextToSpeechProvider> GetAvailableProviders();
    }
}
