namespace SSMLEditor
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Messaging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.FileSystem;
    using Orc.Notifications;
    using Orc.ProjectManagement;
    using Orc.SelectionManagement;
    using SSMLEditor.Messaging;
    using SSMLEditor.Providers;

    public class TTSGenerateAllCommandContainer : TTSCommandContainerBase
    {
        private readonly IPleaseWaitService _pleaseWaitService;
        private readonly IFileService _fileService;
        private readonly IMessageMediator _messageMediator;
        private readonly INotificationService _notificationService;

        public TTSGenerateAllCommandContainer(ICommandManager commandManager, IProjectManager projectManager,
            ISelectionManager<ITextToSpeechProvider> ttsProviderSelectionManager,
            IPleaseWaitService pleaseWaitService, IFileService fileService, IMessageMediator messageMediator,
            INotificationService notificationService)
            : base(Commands.TTS.GenerateAll, commandManager, projectManager, ttsProviderSelectionManager)
        {
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => fileService);
            Argument.IsNotNull(() => messageMediator);
            Argument.IsNotNull(() => notificationService);

            _pleaseWaitService = pleaseWaitService;
            _fileService = fileService;
            _messageMediator = messageMediator;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            var project = _projectManager.GetActiveProject<Project>();
            if (project is null)
            {
                return;
            }

            var ttsProvider = _ttsProviderSelectionManager.GetSelectedItem();
            if (ttsProvider is null)
            {
                return;
            }

            using (_pleaseWaitService.PushInScope())
            {
                var languages = project.ProjectRoot.Languages.ToList();
                for (var i = 0; i < languages.Count; i++)
                {
                    _pleaseWaitService.UpdateStatus(i + 1, languages.Count);

                    var language = languages[i];

                    var ssmlContent = language.Content;
                    if (!string.IsNullOrWhiteSpace(ssmlContent))
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
}
