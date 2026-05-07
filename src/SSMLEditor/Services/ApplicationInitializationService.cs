namespace SSMLEditor.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using Catel;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using Catel.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orc.ProjectManagement;
using Orchestra;
using ProjectManagement;
using Orc.Squirrel;
using MethodTimer;
using Fluent;
using SSMLEditor.Views;

public class ApplicationInitializationService : ApplicationInitializationServiceBase
{
    #region Fields
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ApplicationInitializationService));
    private readonly ICommandManager _commandManager;
    private readonly IBusyIndicatorService _busyIndicatorService;

    private readonly IServiceProvider _serviceProvider;
    #endregion

    #region Constructors
    public ApplicationInitializationService(IServiceProvider serviceProvider, ICommandManager commandManager, IBusyIndicatorService busyIndicatorService)
        : base(serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(commandManager);
        ArgumentNullException.ThrowIfNull(busyIndicatorService);

        _serviceProvider = serviceProvider;
        _commandManager = commandManager;
        _busyIndicatorService = busyIndicatorService;
    }
    #endregion

    #region Methods
    public override async Task InitializeBeforeCreatingShellAsync()
    {
        InitializeFonts();
        InitializeCommands();
        InitializeWatchers();

        var tasks = new List<Task>
        {
            Task.Run(ImprovePerformanceAsync),
            Task.Run(CheckForUpdatesAsync)
        };

        await Task.WhenAll(tasks);

        var textToSpeechProviderService = _serviceProvider.GetRequiredService<ITextToSpeechProviderService>();
        await textToSpeechProviderService.LoadAsync();
    }

    public override async Task InitializeAfterCreatingShellAsync()
    {
        var shellWindow = System.Windows.Application.Current.MainWindow as RibbonWindow;

        var windowCommands = new WindowCommands();
        windowCommands.Items.Add(new WindowCommandsView());
        shellWindow.WindowCommands = windowCommands;

        var mainWindowTitleService = _serviceProvider.GetRequiredService<IMainWindowTitleService>();
        mainWindowTitleService.UpdateTitle();

        await base.InitializeAfterCreatingShellAsync();
    }

    public override async Task InitializeAfterShowingShellAsync()
    {
        await base.InitializeAfterShowingShellAsync();

        await LoadProjectAsync();
    }

    private void InitializeFonts()
    {
        Orc.Theming.FontImage.RegisterFont("FontAwesome", new FontFamily(new Uri("pack://application:,,,/SSMLEditor;component/Resources/Fonts/", UriKind.RelativeOrAbsolute), "./#FontAwesome"));
        Orc.Theming.FontImage.DefaultFontFamily = "FontAwesome";
        Orc.Theming.FontImage.DefaultBrush = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
    }

    [Time]
    private async Task ImprovePerformanceAsync()
    {
        Logger.LogInformation("Improving performance");

        UserControl.DefaultCreateWarningAndErrorValidatorForViewModelValue = false;
        UserControl.DefaultSkipSearchingForInfoBarMessageControlValue = true;
    }

    private void InitializeCommands()
    {
        _commandManager.CreateCommandWithGesture(_serviceProvider, typeof(Commands.Project), nameof(Commands.Project.Close));
        _commandManager.CreateCommandWithGesture(_serviceProvider, typeof(Commands.Project), nameof(Commands.Project.Open));
        _commandManager.CreateCommandWithGesture(_serviceProvider, typeof(Commands.Project), nameof(Commands.Project.Save));

        _commandManager.CreateCommandWithGesture(_serviceProvider, typeof(Commands.Providers), nameof(Commands.Providers.Manage));

        _commandManager.CreateCommandWithGesture(_serviceProvider, typeof(Commands.TTS), nameof(Commands.TTS.Generate));
        _commandManager.CreateCommandWithGesture(_serviceProvider, typeof(Commands.TTS), nameof(Commands.TTS.GenerateAll));

        _commandManager.CreateCommandWithGesture(_serviceProvider, typeof(Commands.Settings), nameof(Commands.Settings.General));

        _commandManager.CreateCommandWithGesture(_serviceProvider, typeof(Commands.Help), nameof(Commands.Help.About));
    }

    private void InitializeWatchers()
    {
        _ = _serviceProvider.GetRequiredService<RecentlyUsedItemsProjectWatcher>();
        _ = _serviceProvider.GetRequiredService<MainWindowTitleProjectWatcher>();
        _ = _serviceProvider.GetRequiredService<ProjectManagementCloseApplicationWatcher>();
    }

    [Time]
    private async Task CheckForUpdatesAsync()
    {
        Logger.LogInformation("Checking for updates");

        var updateService = _serviceProvider.GetRequiredService<IUpdateService>();
        await updateService.InitializeAsync(SSMLEditor.Settings.Application.AutomaticUpdates.AvailableChannels, 
            SSMLEditor.Settings.Application.AutomaticUpdates.DefaultChannel,
            SSMLEditor.Settings.Application.AutomaticUpdates.CheckForUpdatesDefaultValue);

#pragma warning disable 4014
        // Not dot await, it's a background thread
        updateService.InstallAvailableUpdatesAsync(new SquirrelContext());
#pragma warning restore 4014
    }

    protected async Task LoadProjectAsync()
    {
        using (_busyIndicatorService.PushInScope())
        {
            var projectManager = _serviceProvider.GetRequiredService<IProjectManager>();
            if (projectManager is null)
            {
                Logger.LogError("Failed to resolve project manager");
                throw new InvalidOperationException("Failed to resolve project manager");
            }

            await projectManager.InitializeAsync();
        }
    }
    #endregion
}
