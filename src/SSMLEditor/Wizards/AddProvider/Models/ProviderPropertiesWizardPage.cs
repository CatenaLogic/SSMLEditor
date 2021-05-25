namespace SSMLEditor.Wizards.AddProvider
{
    using System.Collections.Generic;
    using Catel;
    using Orc.Wizard;
    using SSMLEditor.Providers;
    using SSMLEditor.Services;

    public class ProviderPropertiesWizardPage : WizardPageBase
    {
        public ProviderPropertiesWizardPage(ITextToSpeechProviderService textToSpeechProviderService)
        {
            Argument.IsNotNull(() => textToSpeechProviderService);

            Title = "Properties";
            Description = "Update the provider properties";
        }

        public List<TtsProperty> Properties { get; set; }
    }
}
