namespace SSMLEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Catel.Reflection;
    using Catel.Services;
    using Orc.ProjectManagement;
    using Orc.SelectionManagement;
    using Orchestra.ViewModels;
    using SSMLEditor.Providers;
    using SSMLEditor.Services;

    public class RibbonViewModel : ViewModelBase
    {
        private readonly IProjectManager _projectManager;
        private readonly ISelectionManager<ITextToSpeechProvider> _textToSpeechProviderSelectionManager;
        private readonly ITextToSpeechProviderService _textToSpeechProviderService;
        private readonly IUIVisualizerService _uiVisualizerService;

        public RibbonViewModel(IUIVisualizerService uiVisualizerService, IProjectManager projectManager,
            ISelectionManager<ITextToSpeechProvider> textToSpeechProviderSelectionManager, ITextToSpeechProviderService textToSpeechProviderService)
        {
            ArgumentNullException.ThrowIfNull(uiVisualizerService);
            ArgumentNullException.ThrowIfNull(projectManager);
            ArgumentNullException.ThrowIfNull(textToSpeechProviderSelectionManager);
            ArgumentNullException.ThrowIfNull(textToSpeechProviderSelectionManager);

            _uiVisualizerService = uiVisualizerService;
            _projectManager = projectManager;
            _textToSpeechProviderSelectionManager = textToSpeechProviderSelectionManager;
            _textToSpeechProviderService = textToSpeechProviderService;

            ShowKeyboardMappings = new TaskCommand(OnShowKeyboardMappingsExecuteAsync);

            Title = AssemblyHelper.GetEntryAssembly().Title();
        }

        public Project Project { get; private set; }

        public List<ITextToSpeechProvider> AvailableProviders { get; private set; }

        public ITextToSpeechProvider SelectedProvider { get; set; }

        #region Commands
        public TaskCommand ShowKeyboardMappings { get; private set; }

        private async Task OnShowKeyboardMappingsExecuteAsync()
        {
            await _uiVisualizerService.ShowDialogAsync<KeyboardMappingsCustomizationViewModel>();
        }
        #endregion

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            AvailableProviders = _textToSpeechProviderService.Providers.ToList();
            SelectedProvider = _textToSpeechProviderSelectionManager.GetSelectedItem() ?? AvailableProviders.FirstOrDefault();

            _projectManager.ProjectActivatedAsync += OnProjectActivatedAsync;
            _textToSpeechProviderSelectionManager.SelectionChanged += OnTextToSpeechProviderSelectionManagerSelectionChanged;
        }

        protected override async Task CloseAsync()
        {
            _projectManager.ProjectActivatedAsync -= OnProjectActivatedAsync;
            _textToSpeechProviderSelectionManager.SelectionChanged -= OnTextToSpeechProviderSelectionManagerSelectionChanged;

            await base.CloseAsync();
        }

        private Task OnProjectActivatedAsync(object sender, ProjectUpdatedEventArgs e)
        {
            Project = (Project)e.NewProject;

            return TaskHelper.Completed;
        }

        private void OnTextToSpeechProviderSelectionManagerSelectionChanged(object sender, SelectionChangedEventArgs<ITextToSpeechProvider> e)
        {
            SelectedProvider = _textToSpeechProviderSelectionManager.GetSelectedItem();
        }

        private void OnSelectedProviderChanged()
        {
            _textToSpeechProviderSelectionManager.Replace(SelectedProvider);
        }
    }
}
