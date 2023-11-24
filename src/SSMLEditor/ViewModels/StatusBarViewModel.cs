namespace SSMLEditor.ViewModels
{
    using System.Threading.Tasks;
    using Catel.MVVM;
    using Orc.ProjectManagement;
    using Orchestra;
    using Orc.Squirrel;
    using System;
    using Catel.Configuration;

    public class StatusBarViewModel : ViewModelBase
    {
        private readonly IProjectManager _projectManager;
        private readonly IConfigurationService _configurationService;
        private readonly IUpdateService _updateService;

        public StatusBarViewModel(IProjectManager projectManager, IConfigurationService configurationService, IUpdateService updateService)
        {
            ArgumentNullException.ThrowIfNull(projectManager);
            ArgumentNullException.ThrowIfNull(configurationService);
            ArgumentNullException.ThrowIfNull(updateService);

            _projectManager = projectManager;
            _configurationService = configurationService;
            _updateService = updateService;
        }

        public string ReceivingAutomaticUpdates { get; private set; }
        public bool IsUpdatedInstalled { get; private set; }
        public string Version { get; private set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _configurationService.ConfigurationChanged += OnConfigurationChanged;
            _updateService.UpdateInstalled += OnUpdateInstalled;
            _projectManager.ProjectActivatedAsync += OnProjectActivatedAsync;

            IsUpdatedInstalled = _updateService.IsUpdatedInstalled;
            Version = VersionHelper.GetCurrentVersion();

            UpdateAutoUpdateInfo();
        }

        protected override async Task CloseAsync()
        {
            _configurationService.ConfigurationChanged -= OnConfigurationChanged;
            _updateService.UpdateInstalled -= OnUpdateInstalled;
            _projectManager.ProjectActivatedAsync -= OnProjectActivatedAsync;

            await base.CloseAsync();
        }

        private void OnConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            if (e.Key.Contains("Updates"))
            {
                UpdateAutoUpdateInfo();
            }
        }

        private void OnUpdateInstalled(object sender, EventArgs e)
        {
            IsUpdatedInstalled = _updateService.IsUpdatedInstalled;
        }

        private void UpdateAutoUpdateInfo()
        {
            var updateInfo = string.Empty;

            var checkForUpdates = _updateService.IsCheckForUpdatesEnabled;
            if (!_updateService.IsUpdateSystemAvailable || !checkForUpdates)
            {
                updateInfo = "Automatic updates are disabled";
            }
            else
            {
                var channel = _updateService.CurrentChannel.Name;
                updateInfo = string.Format("Automatic updates are enabled for {0} versions", channel.ToLower());
            }

            ReceivingAutomaticUpdates = updateInfo;
        }

        private Task OnProjectActivatedAsync(object sender, ProjectUpdatedEventArgs args)
        {
            var activeProject = args.NewProject;
            if (activeProject is null)
            {
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
