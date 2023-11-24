namespace SSMLEditor.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Orc.ProjectManagement;

    public class MainViewModel : ViewModelBase
    {
        private readonly IProjectManager _projectManager;

        public MainViewModel(IProjectManager projectManager)
        {
            ArgumentNullException.ThrowIfNull(projectManager);

            _projectManager = projectManager;
        }

        //[Model]
        //[Expose(nameof(Project.Text))]
        public Project Project { get; set; }

        protected override Task InitializeAsync()
        {
            _projectManager.ProjectActivationAsync += OnProjectActivationAsync;

            return base.InitializeAsync();
        }

        protected override Task OnClosedAsync(bool? result)
        {
            _projectManager.ProjectActivationAsync -= OnProjectActivationAsync;

            return base.OnClosedAsync(result);
        }

        private async Task OnProjectActivationAsync(object sender, ProjectUpdatingCancelEventArgs e)
        {
            var newProject = e.NewProject as Project;
            if (newProject is null)
            {
                return;
            }

            Project = newProject;
        }

        protected override Task CloseAsync()
        {
            _projectManager.ProjectActivationAsync -= OnProjectActivationAsync;

            return base.CloseAsync();
        }
    }
}
