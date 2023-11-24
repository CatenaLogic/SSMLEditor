namespace SSMLEditor
{
    using System;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Orc.ProjectManagement;

    public abstract class ProjectCommandContainerBase : CommandContainerBase
    {
        protected readonly ICommandManager _commandManager;
        protected readonly IProjectManager _projectManager;

        protected ProjectCommandContainerBase(string commandName, ICommandManager commandManager, IProjectManager projectManager)
            : base(commandName, commandManager)
        {
            ArgumentNullException.ThrowIfNull(projectManager);

            _commandManager = commandManager;
            _projectManager = projectManager;

            _projectManager.ProjectActivatedAsync += OnProjectActivatedAsync;
            _projectManager.ProjectClosedAsync += OnProjectClosedAsync;
        }

        private async Task OnProjectActivatedAsync(object sender, ProjectUpdatedEventArgs e)
        {
            await OnProjectActivatedAsync(e.OldProject as Project, e.NewProject as Project);

            InvalidateCommand();
        }

        private async Task OnProjectClosedAsync(object sender, ProjectEventArgs e)
        {
            await OnProjectClosedAsync(e.Project as Project);

            InvalidateCommand();
        }

        protected virtual async Task OnProjectActivatedAsync(Project oldProject, Project newProject)
        {
        }

        protected virtual async Task OnProjectClosedAsync(Project project)
        {
        }

        protected override bool CanExecute(object parameter)
        {
            if (_projectManager.ActiveProject is null)
            {
                return false;
            }

            return base.CanExecute(parameter);
        }
    }
}
