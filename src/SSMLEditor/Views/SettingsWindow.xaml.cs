namespace SSMLEditor.Views;

using System;
using Catel.Services;
using Catel.Windows;
using ViewModels;

public partial class SettingsWindow : DataWindow
{
    public SettingsWindow(SettingsViewModel viewModel, IServiceProvider serviceProvider, IWrapControlService wrapControlService, ILanguageService languageService)
        : base(viewModel, serviceProvider, wrapControlService, languageService)
    {
        InitializeComponent();
    }
}
