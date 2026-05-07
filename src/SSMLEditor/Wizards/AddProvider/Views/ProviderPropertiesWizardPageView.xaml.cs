namespace SSMLEditor.Wizards.AddProvider.Views;

using System;
using Catel.MVVM;
using Catel.Services;

public partial class ProviderPropertiesWizardPageView
{
    public ProviderPropertiesWizardPageView(IServiceProvider serviceProvider, IViewModelWrapperService viewModelWrapperService, IDataContextSubscriptionService dataContextSubscriptionService)
        : base(serviceProvider, viewModelWrapperService, dataContextSubscriptionService)
    {
        InitializeComponent();
    }
}
