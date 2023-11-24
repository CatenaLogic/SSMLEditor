namespace SSMLEditor.Wizards.AddProvider.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Data;
    using Orc.Wizard;
    using SSMLEditor.Providers;
    using SSMLEditor.Services;

    public class ProviderWizardPageViewModel : WizardPageViewModelBase<ProviderWizardPage>
    {
        public ProviderWizardPageViewModel(ProviderWizardPage wizardPage, 
            ITextToSpeechProviderService textToSpeechProviderService)
            : base(wizardPage)
        {
            ArgumentNullException.ThrowIfNull(textToSpeechProviderService);

            Providers = textToSpeechProviderService.GetAvailableProviders().ToList();
            SelectedProvider = Providers.FirstOrDefault();
        }

        public List<ITextToSpeechProvider> Providers { get; private set; }

        public ITextToSpeechProvider SelectedProvider { get; set; }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            base.ValidateFields(validationResults);

            if (SelectedProvider is null)
            {
                validationResults.Add(FieldValidationResult.CreateError(nameof(SelectedProvider), "Select a provider"));
            }
        }
    }
}
