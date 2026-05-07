namespace SSMLEditor.Views;

using System;
using Catel.Services;

public partial class ManageProvidersWindow
{
    public ManageProvidersWindow(IServiceProvider serviceProvider, IWrapControlService wrapControlService, ILanguageService languageService)
        : base(serviceProvider, wrapControlService, languageService)
    {
        InitializeComponent();
    }
}
