namespace SSMLEditor.Wizards.AddProvider
{
    using Catel.IoC;
    using Catel.Logging;
    using Orc.Wizard;
    using SSMLEditor.Providers;

    public class AddProviderWizard : WizardBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public AddProviderWizard(ITypeFactory typeFactory)
            : base(typeFactory)
        {
            Title = "Add provider"; 

            this.AddPage<ProviderWizardPage>();
            this.AddPage<ProviderPropertiesWizardPage>();
            this.AddPage<SummaryWizardPage>();

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
}
