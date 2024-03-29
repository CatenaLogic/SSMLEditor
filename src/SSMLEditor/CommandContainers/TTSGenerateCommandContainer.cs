﻿namespace SSMLEditor
{
    using System.Threading.Tasks;
    using Catel.Messaging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.FileSystem;
    using Orc.Notifications;
    using Orc.ProjectManagement;
    using Orc.SelectionManagement;
    using SSMLEditor.Providers;

    public class TTSGenerateCommandContainer : TTSCommandContainerBase
    {
        private readonly ISelectionManager<Language> _languageSelectionManager;

        public TTSGenerateCommandContainer(ICommandManager commandManager, IProjectManager projectManager,
            ISelectionManager<ITextToSpeechProvider> ttsProviderSelectionManager, ISelectionManager<Language> languageSelectionManager,
            IBusyIndicatorService busyIndicatorService, IFileService fileService, IMessageMediator messageMediator,
            INotificationService notificationService)
            : base(Commands.TTS.Generate, commandManager, projectManager, ttsProviderSelectionManager, busyIndicatorService, fileService, messageMediator, notificationService)
        {
            _languageSelectionManager = languageSelectionManager;
        }

        public override async Task ExecuteAsync(object parameter)
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

            var language = _languageSelectionManager.GetSelectedItem();
            if (language is null)
            {
                return;
            }

            await GenerateLanguageAsync(ttsProvider, project, language);
        }
    }
}
