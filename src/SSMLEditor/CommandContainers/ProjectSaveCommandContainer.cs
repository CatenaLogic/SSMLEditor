namespace SSMLEditor;

using System;
using System.Threading.Tasks;
using Catel.MVVM;
using Orc.ProjectManagement;

public class ProjectSaveCommandContainer : ProjectCommandContainerBase
{
    public ProjectSaveCommandContainer(ICommandManager commandManager, IProjectManager projectManager, IServiceProvider serviceProvider)
        : base(Commands.Project.Save, commandManager, projectManager, serviceProvider)
    {
    }

    public override async Task ExecuteAsync(object parameter)
    {
        await base.ExecuteAsync(parameter);

        var project = _projectManager.GetActiveProject<Project>();
        if (project is not null)
        {
            await _projectManager.SaveAsync(project);
        }
    }
}
