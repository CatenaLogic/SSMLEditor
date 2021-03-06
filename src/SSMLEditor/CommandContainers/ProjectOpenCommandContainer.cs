namespace SSMLEditor
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.FileSystem;
    using Orc.ProjectManagement;

    public class ProjectOpenCommandContainer : ProjectCommandContainerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IFileService _fileService;
        private readonly IOpenFileService _openFileService;
        private readonly IPleaseWaitService _pleaseWaitService;

        public ProjectOpenCommandContainer(ICommandManager commandManager, IProjectManager projectManager, IOpenFileService openFileService,
            IFileService fileService, IPleaseWaitService pleaseWaitService)
            : base(Commands.Project.Open, commandManager, projectManager)
        {
            Argument.IsNotNull(() => openFileService);
            Argument.IsNotNull(() => fileService);
            Argument.IsNotNull(() => pleaseWaitService);

            _openFileService = openFileService;
            _fileService = fileService;
            _pleaseWaitService = pleaseWaitService;
        }

        protected override bool CanExecute(object parameter)
        {
            return true;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            try
            {
                var location = parameter as string;

                if (string.IsNullOrWhiteSpace(location) || 
                    !_fileService.Exists(location))
                {
                    var result = await _openFileService.DetermineFileAsync(new DetermineOpenFileContext
                    {
                        Filter = "Project Files (*.ssmlx)|*.ssmlx",
                        IsMultiSelect = false
                    });

                    if (result.Result)
                    {
                        location = result.FileName;
                    }
                }

                if (!string.IsNullOrWhiteSpace(location))
                {
                    using (_pleaseWaitService.PushInScope())
                    {
                        await _projectManager.LoadAsync(location);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to open file");
            }
        }
    }
}
