﻿namespace SSMLEditor.Wizards.AddProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Orc.Wizard;
    using SSMLEditor.Providers;
    using SSMLEditor.Services;

    public class ProviderWizardPage : WizardPageBase
    {
        public ProviderWizardPage(ITextToSpeechProviderService textToSpeechProviderService)
        {
            ArgumentNullException.ThrowIfNull(textToSpeechProviderService);

            Providers = textToSpeechProviderService.GetAvailableProviders().ToList();
            SelectedProvider = Providers.FirstOrDefault();

            Title = "Provider";
            Description = "Select the provider type";
        }

        public List<ITextToSpeechProvider> Providers { get; private set; }

        public ITextToSpeechProvider SelectedProvider { get; set; }

        public override ISummaryItem GetSummary()
        {
            return new SummaryItem
            {
                Title = "Provider",
                Summary = SelectedProvider.Name
            };
        }
    }
}
