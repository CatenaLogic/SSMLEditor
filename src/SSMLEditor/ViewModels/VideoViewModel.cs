namespace SSMLEditor.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Orc.FileSystem;
    using Orc.ProjectManagement;
    using Orc.SelectionManagement;

    public class VideoViewModel : ViewModelBase
    {
        private readonly IProjectManager _projectManager;
        private readonly ISelectionManager<Language> _languageSelectionManager;
        private readonly IFileService _fileService;

        public VideoViewModel(IProjectManager projectManager, ISelectionManager<Language> languageSelectionManager,
            IFileService fileService)
        {
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => languageSelectionManager);
            Argument.IsNotNull(() => fileService);

            _projectManager = projectManager;
            _languageSelectionManager = languageSelectionManager;
            _fileService = fileService;

            Play = new TaskCommand(OnPlayExecuteAsync, OnPlayCanExecute);
            Pause = new TaskCommand(OnPauseExecuteAsync, OnPauseCanExecute);
        }

        public override string Title { get { return "Video"; } }

        public Uri VideoUri { get; private set; }

        public double Position { get; set; }

        public bool IsPlaying { get; private set; }

        #region Commands
        public TaskCommand Play { get; private set; }

        private bool OnPlayCanExecute()
        {
            if (VideoUri is null)
            {
                return false;
            }

            if (IsPlaying)
            {
                return false;
            }
            
            return true;
        }

        private async Task OnPlayExecuteAsync()
        {
            IsPlaying = true;
        }

        public TaskCommand Pause { get; private set; }

        private bool OnPauseCanExecute()
        {
            if (VideoUri is null)
            {
                return false;
            }

            if (!IsPlaying)
            {
                return false;
            }

            return true;
        }

        private async Task OnPauseExecuteAsync()
        {
            IsPlaying = false;
        }

        #endregion

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _projectManager.ProjectActivatedAsync += OnProjectManagerProjectActivedAsync;
            _projectManager.ProjectClosedAsync += OnProjectManagerProjectClosedAsync;

            UpdateProject();
        }

        protected override async Task CloseAsync()
        {
            _projectManager.ProjectActivatedAsync -= OnProjectManagerProjectActivedAsync;
            _projectManager.ProjectClosedAsync -= OnProjectManagerProjectClosedAsync;

            await base.CloseAsync();
        }

        private async Task OnProjectManagerProjectActivedAsync(object sender, ProjectUpdatedEventArgs e)
        {
            UpdateProject();
        }

        private async Task OnProjectManagerProjectClosedAsync(object sender, ProjectEventArgs e)
        {
            UpdateProject();
        }

        private void UpdateProject()
        {
            Uri videoUri = null;

            var project = _projectManager.GetActiveProject<Project>();
            if (project is not null)
            {
                var videoFileName = project.GetFullPath(project.ProjectRoot.Video);

                if (_fileService.Exists(videoFileName))
                {
                    videoUri = new Uri(videoFileName, UriKind.RelativeOrAbsolute);
                }
            }

            VideoUri = videoUri;

            if (IsPlaying && videoUri is null)
            {
                IsPlaying = false;
            }
        }
    }
}
