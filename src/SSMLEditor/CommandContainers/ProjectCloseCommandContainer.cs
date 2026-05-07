namespace SSMLEditor;

using System;
using System.Threading.Tasks;
using Catel.MVVM;
using Orc.ProjectManagement;

public class ProjectCloseCommandContainer : ProjectCommandContainerBase
{
    public ProjectCloseCommandContainer(ICommandManager commandManager, IProjectManager projectManager, IServiceProvider serviceProvider)
        : base(Commands.Project.Close, commandManager, projectManager, serviceProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        await _projectManager.CloseActiveProjectAsync();
    }
}
