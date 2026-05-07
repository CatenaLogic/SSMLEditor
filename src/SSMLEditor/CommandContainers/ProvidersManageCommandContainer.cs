namespace SSMLEditor;

using System;
using System.Threading.Tasks;
using Catel.MVVM;
using Catel.Services;
using Orc.Wizard;
using SSMLEditor.ViewModels;

public class ProvidersManageCommandContainer : CommandContainerBase
{
    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly IWizardService _wizardService;

    public ProvidersManageCommandContainer(ICommandManager commandManager, IUIVisualizerService uiVisualizerService,
        IWizardService wizardService, IServiceProvider serviceProvider)
        : base(Commands.Providers.Manage, commandManager, serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(uiVisualizerService);
        ArgumentNullException.ThrowIfNull(wizardService);

        _uiVisualizerService = uiVisualizerService;
        _wizardService = wizardService;
    }

    public override async Task ExecuteAsync(object parameter)
    {
        await _uiVisualizerService.ShowDialogAsync<ManageProvidersViewModel>();
    }
}
