namespace SSMLEditor
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Services;
    using Orc.ProjectManagement;
    using Orchestra;
    using System;

    public class ProjectManagementCloseApplicationWatcher : CloseApplicationWatcherBase
    {
        private readonly IProjectManager _projectManager;
        private readonly IPleaseWaitService _pleaseWaitService;

        public ProjectManagementCloseApplicationWatcher(IProjectManager projectManager, IPleaseWaitService pleaseWaitService)
        {
            ArgumentNullException.ThrowIfNull(projectManager);
            ArgumentNullException.ThrowIfNull(pleaseWaitService);

            _projectManager = projectManager;
            _pleaseWaitService = pleaseWaitService;
        }

        protected override async Task<bool> ClosingAsync()
        {
            using (_pleaseWaitService.PushInScope())
            {
                var projects = _projectManager.Projects.OfType<Project>().OrderByDescending(x => x.IsDirty).ToArray();

                for (var i = 0; i < projects.Length; i++)
                {
                    var project = projects[i];
                    project.ClearIsDirty();
                    var closed = await _projectManager.CloseAsync(project);
                    if (!closed)
                    {
                        return false;
                    }

                    _pleaseWaitService.UpdateStatus(i, projects.Length);
                }
            }

            return await base.ClosingAsync();
        }

        protected override async Task<bool> PrepareClosingAsync()
        {
            //foreach (var project in _projectManager.Projects.OfType<Project>())
            //{
            //    if (!await _saveProjectChangesService.EnsureChangesSavedAsync(project, SaveChangesReason.Closing))
            //    {
            //        return false;
            //    }
            //}

            return await base.PrepareClosingAsync();
        }
    }
}
