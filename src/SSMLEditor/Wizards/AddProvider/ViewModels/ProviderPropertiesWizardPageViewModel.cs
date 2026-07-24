namespace SSMLEditor.Wizards.AddProvider.ViewModels;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orc.Wizard;
using SSMLEditor.Providers;
using SSMLEditor.Wizards.AddProvider;

public class ProviderPropertiesWizardPageViewModel : WizardPageViewModelBase<ProviderPropertiesWizardPage>
{
    public ProviderPropertiesWizardPageViewModel(ProviderPropertiesWizardPage wizardPage, IServiceProvider serviceProvider)
        : base(wizardPage, serviceProvider)
    {

    }

    public string? Name { get; set; }

    public IReadOnlyList<TtsProperty> Properties { get; private set; } = Array.Empty<TtsProperty>();

    protected override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        var providerWizardPage = Wizard!.FindRequiredPageByType<ProviderWizardPage>();

        var selectedProvider = providerWizardPage.SelectedProvider;
        Name = selectedProvider?.Name ?? string.Empty;
        Properties = selectedProvider is not null ? selectedProvider.Properties : Array.Empty<TtsProperty>();
    }

    protected override async Task CloseAsync()
    {
        await base.CloseAsync();
    }
}
