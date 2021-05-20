namespace SSMLEditor.Services
{
    using System.Collections.Generic;
    using SSMLEditor.Providers;

    public class TextToSpeechProviderService : InterfaceFinderServiceBase<ITextToSpeechProvider>, ITextToSpeechProviderService
    {
        public IEnumerable<ITextToSpeechProvider> GetProviders()
        {
            return GetAvailableItems();
        }
    }
}
