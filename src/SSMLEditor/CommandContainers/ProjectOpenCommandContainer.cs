namespace SSMLEditor
{
    using System;
    using System.Threading.Tasks;
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
        private readonly IBusyIndicatorService _busyIndicatorService;

        public ProjectOpenCommandContainer(ICommandManager commandManager, IProjectManager projectManager, IOpenFileService openFileService,
            IFileService fileService, IBusyIndicatorService busyIndicatorService)
            : base(Commands.Project.Open, commandManager, projectManager)
        {
            ArgumentNullException.ThrowIfNull(openFileService);
            ArgumentNullException.ThrowIfNull(fileService);
            ArgumentNullException.ThrowIfNull(busyIndicatorService);

            _openFileService = openFileService;
            _fileService = fileService;
            _busyIndicatorService = busyIndicatorService;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override async Task ExecuteAsync(object parameter)
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
                    using (_busyIndicatorService.PushInScope())
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
