namespace SSMLEditor.Wizards.AddProvider.Views;

using System;
using Catel.MVVM;
using Catel.Services;

public partial class ProviderWizardPageView
{
    public ProviderWizardPageView(IServiceProvider serviceProvider, IViewModelWrapperService viewModelWrapperService, IDataContextSubscriptionService dataContextSubscriptionService)
        : base(serviceProvider, viewModelWrapperService, dataContextSubscriptionService)
    {
        InitializeComponent();
    }
}
