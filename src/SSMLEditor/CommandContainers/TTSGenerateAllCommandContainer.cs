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
        public TTSGenerateAllCommandContainer(ICommandManager commandManager, IProjectManager projectManager,
            ISelectionManager<ITextToSpeechProvider> ttsProviderSelectionManager,
            IPleaseWaitService pleaseWaitService, IFileService fileService, IMessageMediator messageMediator,
            INotificationService notificationService)
            : base(Commands.TTS.GenerateAll, commandManager, projectManager, ttsProviderSelectionManager,
                  pleaseWaitService, fileService, messageMediator, notificationService)
        {
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

                    await GenerateLanguageAsync(ttsProvider, project, language);
                }
            }
        }
    }
}
