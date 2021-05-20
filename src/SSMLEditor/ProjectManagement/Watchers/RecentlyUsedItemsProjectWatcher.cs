namespace SSMLEditor.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Orc.ProjectManagement;
    using Orchestra.Models;
    using Orchestra.Services;

    public class RecentlyUsedItemsProjectWatcher : ProjectWatcherBase
    {
        private readonly IRecentlyUsedItemsService _recentlyUsedItemsService;

        public RecentlyUsedItemsProjectWatcher(IProjectManager projectManager, IRecentlyUsedItemsService recentlyUsedItemsService)
            : base(projectManager)
        {
            Argument.IsNotNull(() => recentlyUsedItemsService);

            _recentlyUsedItemsService = recentlyUsedItemsService;
        }

        protected override Task OnLoadedAsync(IProject project)
        {
            _recentlyUsedItemsService.AddItem(new RecentlyUsedItem(project.Location, DateTime.Now));

            return base.OnLoadedAsync(project);
        }
    }
}
