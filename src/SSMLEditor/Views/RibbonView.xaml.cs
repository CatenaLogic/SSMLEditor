namespace SSMLEditor.Views;

using Catel.MVVM;
using Orchestra;

public partial class RibbonView
{
    [Catel.InjectedService]
    private readonly IAboutService _aboutService = null!;

    partial void OnInitializedComponent()
    {
        ribbon.AddAboutButton(_aboutService);
    }
}
