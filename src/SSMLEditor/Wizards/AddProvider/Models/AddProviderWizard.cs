namespace SSMLEditor.Wizards.AddProvider;

using System;
using Orc.Wizard;
using SSMLEditor.Providers;

public class AddProviderWizard : WizardBase
{
    public AddProviderWizard(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        Title = "Add provider"; 

        this.AddPage<ProviderWizardPage>(serviceProvider);
        this.AddPage<ProviderPropertiesWizardPage>(serviceProvider);
        this.AddPage<SummaryWizardPage>(serviceProvider);

        MinSize = new System.Windows.Size(800, 600);
        MaxSize = new System.Windows.Size(1000, 800);
        ResizeMode = System.Windows.ResizeMode.CanResize;
    }

    public ITextToSpeechProvider Provider
    {
        get
        {
            var wizardPage = this.FindPageByType<ProviderWizardPage>();
            return wizardPage.SelectedProvider;
        }
    }
}
