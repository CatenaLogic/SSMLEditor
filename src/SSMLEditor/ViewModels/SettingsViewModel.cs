namespace SSMLEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel.Configuration;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.Squirrel;
    using Orchestra.Services;

    public class SettingsViewModel : ViewModelBase
    {
        private readonly IConfigurationService _configurationService;
        private readonly IManageAppDataService _manageAppDataService;
        private readonly IUpdateService _updateService;
        private readonly IOpenFileService _openFileService;

        public SettingsViewModel(IConfigurationService configurationService, IManageAppDataService manageAppDataService, 
            IUpdateService updateService, IOpenFileService openFileService)
        {
            ArgumentNullException.ThrowIfNull(configurationService);
            ArgumentNullException.ThrowIfNull(manageAppDataService);
            ArgumentNullException.ThrowIfNull(updateService);
            ArgumentNullException.ThrowIfNull(openFileService);

            _configurationService = configurationService;
            _manageAppDataService = manageAppDataService;
            _updateService = updateService;
            _openFileService = openFileService;

            Title = "Settings";
        }

        public bool IsUpdateSystemAvailable { get; private set; }
        public bool CheckForUpdates { get; set; }
        public List<UpdateChannel> AvailableUpdateChannels { get; private set; }
        public UpdateChannel UpdateChannel { get; set; }

        #region Commands
        #endregion

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            IsUpdateSystemAvailable = _updateService.IsUpdateSystemAvailable;
            CheckForUpdates = _updateService.CheckForUpdates;
            AvailableUpdateChannels = new List<UpdateChannel>(_updateService.AvailableChannels);
            UpdateChannel = _updateService.CurrentChannel;

            //CustomEditor = _configurationService.GetRoamingValue<string>(Configuration.CustomEditor);
            //AutoSaveEditor = _configurationService.GetRoamingValue(Configuration.AutoSaveEditor, Configuration.AutoSaveEditorDefaultValue);
        }

        protected override async Task<bool> SaveAsync()
        {
            _updateService.CheckForUpdates = CheckForUpdates;
            _updateService.CurrentChannel = UpdateChannel;

            //_configurationService.SetRoamingValue(Configuration.CustomEditor, CustomEditor);
            //_configurationService.SetRoamingValue(Configuration.AutoSaveEditor, AutoSaveEditor);

            return await base.SaveAsync();
        }
    }
}
