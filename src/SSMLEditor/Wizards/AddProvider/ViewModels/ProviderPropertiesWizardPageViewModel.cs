namespace SSMLEditor.Wizards.AddProvider.ViewModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Orc.Wizard;
    using SSMLEditor.Providers;
    using SSMLEditor.Wizards.AddProvider;

    public class ProviderPropertiesWizardPageViewModel : WizardPageViewModelBase<ProviderPropertiesWizardPage>
    {
        public ProviderPropertiesWizardPageViewModel(ProviderPropertiesWizardPage wizardPage)
            : base(wizardPage)
        {

        }

        public string Name { get; set; }

        public List<TtsProperty> Properties { get; private set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var providerWizardPage = Wizard.FindPageByType<ProviderWizardPage>();

            Name = providerWizardPage.SelectedProvider.Name;
            Properties = providerWizardPage.SelectedProvider.Properties;
        }

        protected override async Task CloseAsync()
        {
            await base.CloseAsync();
        }
    }
}
