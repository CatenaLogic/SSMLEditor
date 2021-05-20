namespace SSMLEditor.Services
{
    using System.Collections.Generic;
    using SSMLEditor.Providers;

    internal interface ITextToSpeechProviderService
    {
        IEnumerable<ITextToSpeechProvider> GetProviders();
    }
}
