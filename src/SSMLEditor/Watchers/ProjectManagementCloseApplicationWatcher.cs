﻿namespace SSMLEditor
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
        private readonly IBusyIndicatorService _busyIndicatorService;

        public ProjectManagementCloseApplicationWatcher(IProjectManager projectManager, IBusyIndicatorService busyIndicatorService)
        {
            ArgumentNullException.ThrowIfNull(projectManager);
            ArgumentNullException.ThrowIfNull(busyIndicatorService);

            _projectManager = projectManager;
            _busyIndicatorService = busyIndicatorService;
        }

        protected override async Task<bool> ClosingAsync()
        {
            using (_busyIndicatorService.PushInScope())
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

                    _busyIndicatorService.UpdateStatus(i, projects.Length);
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
