namespace SSMLEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Orc.SelectionManagement;
    using SSMLEditor.Providers;
    using SSMLEditor.Services;

    public class TextToSpeechProviderViewModel : ViewModelBase
    {
        private readonly ISelectionManager<ITextToSpeechProvider> _textToSpeechProviderSelectionManager;
        private readonly ITextToSpeechProviderService _textToSpeechProviderService;

        public TextToSpeechProviderViewModel(ISelectionManager<ITextToSpeechProvider> textToSpeechProviderSelectionManager, ITextToSpeechProviderService textToSpeechProviderService)
        {
            Argument.IsNotNull(() => textToSpeechProviderSelectionManager);
            Argument.IsNotNull(() => textToSpeechProviderService);

            _textToSpeechProviderSelectionManager = textToSpeechProviderSelectionManager;
            _textToSpeechProviderService = textToSpeechProviderService;
        }

        public List<ITextToSpeechProvider> AvailableProviders { get; private set; }

        public ITextToSpeechProvider SelectedProvider { get; set; }

        public List<TtsProperty> Properties { get; private set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            AvailableProviders = _textToSpeechProviderService.GetAvailableProviders().ToList();
            SelectedProvider = _textToSpeechProviderSelectionManager.GetSelectedItem() ?? AvailableProviders.FirstOrDefault();

            _textToSpeechProviderSelectionManager.SelectionChanged += OnTextToSpeechProviderSelectionManagerSelectionChanged;

            Update();
        }

        protected override async Task CloseAsync()
        {
            _textToSpeechProviderSelectionManager.SelectionChanged -= OnTextToSpeechProviderSelectionManagerSelectionChanged;

            await base.CloseAsync();
        }

        private void OnTextToSpeechProviderSelectionManagerSelectionChanged(object sender, SelectionChangedEventArgs<ITextToSpeechProvider> e)
        {
            Update();
        }

        private void OnSelectedProviderChanged()
        {
            _textToSpeechProviderSelectionManager.Replace(SelectedProvider);
        }

        private void Update()
        {
            var properties = new List<TtsProperty>();

            var provider = _textToSpeechProviderSelectionManager.GetSelectedItem();
            if (provider is not null)
            {
                properties.AddRange(provider.Properties);
            }

            Properties = properties;
        }
    }
}
