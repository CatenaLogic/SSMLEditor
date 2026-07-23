namespace SSMLEditor.ProjectManagement;

using System;
using System.Threading.Tasks;
using Catel.IoC;
using Orc.ProjectManagement;
using Services;

public class MainWindowTitleProjectWatcher : ProjectWatcherBase, IConstructAtStartup
{
    private readonly IMainWindowTitleService _mainWindowTitleService;

    public MainWindowTitleProjectWatcher(IProjectManager projectManager, IMainWindowTitleService mainWindowTitleService)
        : base(projectManager)
    {
        ArgumentNullException.ThrowIfNull(mainWindowTitleService);

        _mainWindowTitleService = mainWindowTitleService;
    }

    protected override Task OnActivatedAsync(IProject? oldProject, IProject? newProject)
    {
        _mainWindowTitleService.UpdateTitle();

        return base.OnActivatedAsync(oldProject, newProject);
    }
}
