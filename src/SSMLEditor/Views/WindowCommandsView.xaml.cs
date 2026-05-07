namespace SSMLEditor.Views;

using System;
using Catel.MVVM;
using Catel.Services;

public partial class WindowCommandsView
{
    public WindowCommandsView(IServiceProvider serviceProvider, IViewModelWrapperService viewModelWrapperService, IDataContextSubscriptionService dataContextSubscriptionService)
        : base(serviceProvider, viewModelWrapperService, dataContextSubscriptionService)
    {
        InitializeComponent();
    }
}
