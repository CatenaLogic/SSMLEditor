namespace SSMLEditor.Views;

using Catel.Windows;

public partial class SettingsWindow
{
    partial void OnInitializingComponent()
    {
        Mode = DataWindowMode.OkCancel;
    }
}
