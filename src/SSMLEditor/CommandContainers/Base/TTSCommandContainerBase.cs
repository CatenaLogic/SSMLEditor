namespace SSMLEditor
{
    using System;
    using System.Threading.Tasks;
    using Catel.Messaging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.FileSystem;
    using Orc.Notifications;
    using Orc.ProjectManagement;
    using Orc.SelectionManagement;
    using SSMLEditor.Messaging;
    using SSMLEditor.Providers;

    public abstract class TTSCommandContainerBase : ProjectCommandContainerBase
    {
        protected readonly ISelectionManager<ITextToSpeechProvider> _ttsProviderSelectionManager;
        protected readonly IPleaseWaitService _pleaseWaitService;
        protected readonly IFileService _fileService;
        protected readonly IMessageMediator _messageMediator;
        protected readonly INotificationService _notificationService;

        private bool _hasSelectedItem;

        protected TTSCommandContainerBase(string commandName, ICommandManager commandManager, IProjectManager projectManager,
            ISelectionManager<ITextToSpeechProvider> ttsProviderSelectionManager, IPleaseWaitService pleaseWaitService, 
            IFileService fileService, IMessageMediator messageMediator, INotificationService notificationService)
            : base(commandName, commandManager, projectManager)
        {
            ArgumentNullException.ThrowIfNull(ttsProviderSelectionManager);
            ArgumentNullException.ThrowIfNull(pleaseWaitService);
            ArgumentNullException.ThrowIfNull(fileService);
            ArgumentNullException.ThrowIfNull(messageMediator);
            ArgumentNullException.ThrowIfNull(notificationService);

            _ttsProviderSelectionManager = ttsProviderSelectionManager;
            _pleaseWaitService = pleaseWaitService;
            _fileService = fileService;
            _messageMediator = messageMediator;
            _notificationService = notificationService;

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
            _hasSelectedItem = _ttsProviderSelectionManager.GetSelectedItem() is not null;
        }

        protected virtual async Task GenerateLanguageAsync(ITextToSpeechProvider ttsProvider, Project project, Language language)
        {
            var ssmlContent = language.Content;
            if (!string.IsNullOrWhiteSpace(ssmlContent))
            {
                using (_pleaseWaitService.PushInScope())
                {
                    _messageMediator.SendMessage(new TTSGenerating(language));

                    try
                    {
                        using (var stream = await ttsProvider.ExecuteAsync(ssmlContent))
                        {
                            stream.Position = 0L;

                            var fileName = project.GetFullAudioPath(language);

                            using (var fileStream = _fileService.Create(fileName))
                            {
                                await stream.CopyToAsync(fileStream);
                                await fileStream.FlushAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _notificationService.ShowErrorNotification($"'{language.ShortName}' processing failed", $"Language '{language.ShortName}' failed:\n{ex.Message}");
                    }

                    _messageMediator.SendMessage(new TTSGenerated(language));
                }
            }
        }
    }
}
