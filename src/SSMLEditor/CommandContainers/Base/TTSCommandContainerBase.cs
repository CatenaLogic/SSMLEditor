namespace SSMLEditor
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Orc.ProjectManagement;
    using Orc.SelectionManagement;
    using SSMLEditor.Providers;

    public abstract class TTSCommandContainerBase : ProjectCommandContainerBase
    {
        protected readonly ISelectionManager<ITextToSpeechProvider> _ttsProviderSelectionManager;

        private bool _hasSelectedItem;

        protected TTSCommandContainerBase(string commandName, ICommandManager commandManager, IProjectManager projectManager, 
            ISelectionManager<ITextToSpeechProvider> ttsProviderSelectionManager)
            : base(commandName, commandManager, projectManager)
        {
            Argument.IsNotNull(() => ttsProviderSelectionManager);

            _ttsProviderSelectionManager = ttsProviderSelectionManager;
            _ttsProviderSelectionManager.SelectionChanged += OnTtsProviderSelectionManagerSelectionChanged;

            UpdateSelectionState();
        }

        private void OnTtsProviderSelectionManagerSelectionChanged(object sender, SelectionChangedEventArgs<ITextToSpeechProvider> e)
        {
            UpdateSelectionState();

            InvalidateCommand();
        }

        protected override bool CanExecute(object parameter)
        {
            if (!base.CanExecute(parameter))
            {
                return false;
            }

            return _hasSelectedItem;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            await _projectManager.CloseAsync();
        }

        private void UpdateSelectionState()
        {
            _hasSelectedItem = _ttsProviderSelectionManager.GetSelectedItem() is null;
        }
    }
}
