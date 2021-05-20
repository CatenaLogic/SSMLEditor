namespace SSMLEditor.Views
{
    using Catel.Windows;
    using ViewModels;

    public partial class SettingsWindow : DataWindow
    {
        public SettingsWindow()
            : this(null)
        {
        }

        public SettingsWindow(SettingsViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
