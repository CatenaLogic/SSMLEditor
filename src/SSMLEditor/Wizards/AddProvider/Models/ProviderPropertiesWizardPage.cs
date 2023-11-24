namespace SSMLEditor.Wizards.AddProvider
{
    using System;
    using System.Collections.Generic;
    using Orc.Wizard;
    using SSMLEditor.Providers;
    using SSMLEditor.Services;

    public class ProviderPropertiesWizardPage : WizardPageBase
    {
        public ProviderPropertiesWizardPage(ITextToSpeechProviderService textToSpeechProviderService)
        {
            ArgumentNullException.ThrowIfNull(textToSpeechProviderService);

            Title = "Properties";
            Description = "Update the provider properties";
        }

        public List<TtsProperty> Properties { get; set; }
    }
}
