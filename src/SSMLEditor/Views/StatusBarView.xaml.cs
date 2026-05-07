namespace SSMLEditor.Views;

using System;
using Catel.MVVM;
using Catel.Services;

public partial class StatusBarView
{
    public StatusBarView(IServiceProvider serviceProvider, IViewModelWrapperService viewModelWrapperService, IDataContextSubscriptionService dataContextSubscriptionService)
        : base(serviceProvider, viewModelWrapperService, dataContextSubscriptionService)
    {
        InitializeComponent();
    }
}
