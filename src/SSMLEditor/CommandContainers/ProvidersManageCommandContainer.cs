namespace SSMLEditor
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.Wizard;
    using SSMLEditor.ViewModels;

    public class ProvidersManageCommandContainer : CommandContainerBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IWizardService _wizardService;

        public ProvidersManageCommandContainer(ICommandManager commandManager, IUIVisualizerService uiVisualizerService,
            IWizardService wizardService)
            : base(Commands.Providers.Manage, commandManager)
        {
            ArgumentNullException.ThrowIfNull(uiVisualizerService);
            ArgumentNullException.ThrowIfNull(wizardService);

            _uiVisualizerService = uiVisualizerService;
            _wizardService = wizardService;
        }

        protected override async Task ExecuteAsync(object parameter)
        {
            await _uiVisualizerService.ShowDialogAsync<ManageProvidersViewModel>();
        }
    }
}
