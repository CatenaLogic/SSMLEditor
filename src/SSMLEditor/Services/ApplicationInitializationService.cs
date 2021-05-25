namespace SSMLEditor.Services
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Catel;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.Threading;
    using Catel.Windows.Controls;
    using Orc.ProjectManagement;
    using Orchestra.Markup;
    using Orchestra.Services;
    using ProjectManagement;
    using Orc.Squirrel;
    using MethodTimer;
    using Fluent;
    using SSMLEditor.Views;
    using Orc.SelectionManagement;
    using SSMLEditor.Providers;

    public class ApplicationInitializationService : ApplicationInitializationServiceBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly ICommandManager _commandManager;
        private readonly IPleaseWaitService _pleaseWaitService;

        private readonly IServiceLocator _serviceLocator;
        #endregion

        #region Constructors
        public ApplicationInitializationService(IServiceLocator serviceLocator, ICommandManager commandManager, IPleaseWaitService pleaseWaitService)
        {
            Argument.IsNotNull(() => serviceLocator);
            Argument.IsNotNull(() => commandManager);
            Argument.IsNotNull(() => pleaseWaitService);

            _serviceLocator = serviceLocator;
            _commandManager = commandManager;
            _pleaseWaitService = pleaseWaitService;
        }
        #endregion

        #region Methods
        public override async Task InitializeBeforeCreatingShellAsync()
        {
            RegisterTypes();
            InitializeFonts();
            InitializeCommands();
            InitializeWatchers();

            await TaskHelper.RunAndWaitAsync(new Func<Task>[] {
                ImprovePerformanceAsync,
                CheckForUpdatesAsync
            });

            var textToSpeechProviderService = _serviceLocator.ResolveType<ITextToSpeechProviderService>();
            await textToSpeechProviderService.LoadAsync();
        }

        public override async Task InitializeAfterCreatingShellAsync()
        {
            var shellWindow = System.Windows.Application.Current.MainWindow as RibbonWindow;

            var windowCommands = new WindowCommands();
            windowCommands.Items.Add(new WindowCommandsView());
            shellWindow.WindowCommands = windowCommands;

            var mainWindowTitleService = _serviceLocator.ResolveType<IMainWindowTitleService>();
            mainWindowTitleService.UpdateTitle();

            await base.InitializeAfterCreatingShellAsync();
        }

        public override async Task InitializeAfterShowingShellAsync()
        {
            await base.InitializeAfterShowingShellAsync();

            await LoadProjectAsync();
        }

        private void RegisterTypes()
        {
            _serviceLocator.RegisterType<ISelectionManager<ITextToSpeechProvider>, SelectionManager<ITextToSpeechProvider>>();
            _serviceLocator.RegisterType<ISelectionManager<Language>, SelectionManager<Language>>();

            _serviceLocator.RegisterType<IProjectSerializerSelector, ProjectSerializerSelector>();
            _serviceLocator.RegisterType<IMainWindowTitleService, MainWindowTitleService>();
            _serviceLocator.RegisterType<IInitialProjectLocationService, InitialProjectLocationService>();
            _serviceLocator.RegisterType<ITextToSpeechProviderService, TextToSpeechProviderService>();
            _serviceLocator.RegisterType<ISsmlConverterService, SsmlConverterService>();

            _serviceLocator.RegisterType<IProjectInitializer, FileProjectInitializer>();
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
            Log.Info("Improving performance");

            UserControl.DefaultCreateWarningAndErrorValidatorForViewModelValue = false;
            UserControl.DefaultSkipSearchingForInfoBarMessageControlValue = true;
        }

        private void InitializeCommands()
        {
            _commandManager.CreateCommandWithGesture(typeof(Commands.Project), nameof(Commands.Project.Close));
            _commandManager.CreateCommandWithGesture(typeof(Commands.Project), nameof(Commands.Project.Open));
            _commandManager.CreateCommandWithGesture(typeof(Commands.Project), nameof(Commands.Project.Save));

            _commandManager.CreateCommandWithGesture(typeof(Commands.Providers), nameof(Commands.Providers.Manage));

            _commandManager.CreateCommandWithGesture(typeof(Commands.TTS), nameof(Commands.TTS.Generate));
            _commandManager.CreateCommandWithGesture(typeof(Commands.TTS), nameof(Commands.TTS.GenerateAll));

            _commandManager.CreateCommandWithGesture(typeof(Commands.Settings), nameof(Commands.Settings.General));

            _commandManager.CreateCommandWithGesture(typeof(Commands.Help), nameof(Commands.Help.About));
        }

        private void InitializeWatchers()
        {
            _serviceLocator.RegisterTypeAndInstantiate<RecentlyUsedItemsProjectWatcher>();
            _serviceLocator.RegisterTypeAndInstantiate<MainWindowTitleProjectWatcher>();
            _serviceLocator.RegisterTypeAndInstantiate<ProjectManagementCloseApplicationWatcher>();
        }

        [Time]
        private async Task CheckForUpdatesAsync()
        {
            Log.Info("Checking for updates");

            var updateService = _serviceLocator.ResolveType<IUpdateService>();
            updateService.Initialize(SSMLEditor.Settings.Application.AutomaticUpdates.AvailableChannels, 
                SSMLEditor.Settings.Application.AutomaticUpdates.DefaultChannel,
                SSMLEditor.Settings.Application.AutomaticUpdates.CheckForUpdatesDefaultValue);

#pragma warning disable 4014
            // Not dot await, it's a background thread
            updateService.InstallAvailableUpdatesAsync(new SquirrelContext());
#pragma warning restore 4014
        }

        protected async Task LoadProjectAsync()
        {
            using (_pleaseWaitService.PushInScope())
            {
                var projectManager = _serviceLocator.ResolveType<IProjectManager>();
                if (projectManager is null)
                {
                    const string error = "Failed to resolve project manager";
                    Log.Error(error);
                    throw new Exception(error);
                }

                await projectManager.InitializeAsync();
            }
        }
        #endregion
    }
}
