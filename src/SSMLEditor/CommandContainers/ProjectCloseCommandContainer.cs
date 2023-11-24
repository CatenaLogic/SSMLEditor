namespace SSMLEditor
{
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Orc.ProjectManagement;

    public class ProjectCloseCommandContainer : ProjectCommandContainerBase
    {
        public ProjectCloseCommandContainer(ICommandManager commandManager, IProjectManager projectManager)
            : base(Commands.Project.Close, commandManager, projectManager)
        {
        }

        public override async Task ExecuteAsync(object parameter)
        {
            await _projectManager.CloseAsync();
        }
    }
}
