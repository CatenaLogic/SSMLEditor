﻿namespace SSMLEditor
{
    using System.IO;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Messaging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.FileSystem;
    using Orc.ProjectManagement;
    using Orc.SelectionManagement;
    using SSMLEditor.Messaging;
    using SSMLEditor.Providers;

    public class TTSGenerateCommandContainer : TTSCommandContainerBase
    {
        private readonly ISelectionManager<Language> _languageSelectionManager;
        private readonly IPleaseWaitService _pleaseWaitService;
        private readonly IFileService _fileService;
        private readonly IMessageMediator _messageMediator;

        public TTSGenerateCommandContainer(ICommandManager commandManager, IProjectManager projectManager,
            ISelectionManager<ITextToSpeechProvider> ttsProviderSelectionManager, ISelectionManager<Language> languageSelectionManager,
            IPleaseWaitService pleaseWaitService, IFileService fileService, IMessageMediator messageMediator)
            : base(Commands.TTS.Generate, commandManager, projectManager, ttsProviderSelectionManager)
        {
            Argument.IsNotNull(() => languageSelectionManager);
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => messageMediator);

            _languageSelectionManager = languageSelectionManager;
            _pleaseWaitService = pleaseWaitService;
            _fileService = fileService;
            _messageMediator = messageMediator;
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

            var language = _languageSelectionManager.GetSelectedItem();
            if (language is null)
            {
                return;
            }

            var ssmlContent = language.Content;
            if (!string.IsNullOrWhiteSpace(ssmlContent))
            {
                using (_pleaseWaitService.PushInScope())
                {
                    _messageMediator.SendMessage(new TTSGenerating(language));

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

                    _messageMediator.SendMessage(new TTSGenerated(language));
                }
            }
        }
    }
}
