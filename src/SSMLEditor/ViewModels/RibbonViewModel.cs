namespace SSMLEditor.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Catel.Reflection;
    using Catel.Services;
    using Catel.Threading;
    using Orc.ProjectManagement;
    using Orchestra.ViewModels;

    public class RibbonViewModel : ViewModelBase
    {
        private readonly IProjectManager _projectManager;
        private readonly IUIVisualizerService _uiVisualizerService;

        public RibbonViewModel(IUIVisualizerService uiVisualizerService, IProjectManager projectManager)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => projectManager);

            _uiVisualizerService = uiVisualizerService;
            _projectManager = projectManager;

            ShowKeyboardMappings = new TaskCommand(OnShowKeyboardMappingsExecuteAsync);

            Title = AssemblyHelper.GetEntryAssembly().Title();
        }

        public Project Project { get; private set; }       

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

            _projectManager.ProjectActivatedAsync += OnProjectActivatedAsync;
        }

        protected override async Task CloseAsync()
        {
            _projectManager.ProjectActivatedAsync -= OnProjectActivatedAsync;

            await base.CloseAsync();
        }

        private Task OnProjectActivatedAsync(object sender, ProjectUpdatedEventArgs e)
        {
            Project = (Project)e.NewProject;

            return TaskHelper.Completed;
        }
    }
}
