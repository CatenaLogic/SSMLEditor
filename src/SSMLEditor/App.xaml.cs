namespace SSMLEditor;

using System;
using System.Globalization;
using System.Windows;
using Catel;
using Catel.Configuration;
using Catel.IoC;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orc;
using Orc.ProjectManagement;
using Orc.SelectionManagement;
using Orchestra;
using Orchestra.Views;
using SSMLEditor.ProjectManagement;
using SSMLEditor.Providers;
using SSMLEditor.Services;
using SSMLEditor.Views;
using Velopack;

public partial class App : Application
{
#pragma warning disable IDISP006 // Implement IDisposable
    private readonly IHost _host;
#pragma warning restore IDISP006 // Implement IDisposable

    public App()
    {
        // Keep here, even though we have it in module initializer. But in case module
        // initializer is not called we still want to initialize velopack.
        VelopackApp.Build().Run();

        var hostBuilder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddCatelCore();
                services.AddCatelMvvm();
                services.AddOrcAutomation();
                services.AddOrcControls();
                services.AddOrcFileSystem();
                services.AddOrcSerializationJson();
                services.AddOrcSquirrel();
                services.AddOrcSquirrelXaml();
                services.AddOrcSystemInfo();
                services.AddOrcTheming();
                services.AddOrchestraCore();
                services.AddOrchestraShellRibbonFluent();

                services.AddSingleton<IAboutInfoService, AboutInfoService>();
                services.AddSingleton<IRibbonService, RibbonService>();
                services.AddSingleton<IApplicationInitializationService, ApplicationInitializationService>();
                services.AddSingleton<ISelectionManager<ITextToSpeechProvider>, SelectionManager<ITextToSpeechProvider>>();
                services.AddSingleton<ISelectionManager<Language>, SelectionManager<Language>>();
                services.AddSingleton<IAnalyzerService, AnalyzerService>();
                services.AddSingleton<IProjectSerializerSelector, ProjectSerializerSelector>();
                services.AddSingleton<IMainWindowTitleService, MainWindowTitleService>();
                services.AddSingleton<IInitialProjectLocationService, SSMLEditor.Services.InitialProjectLocationService>();
                services.AddSingleton<ITextToSpeechProviderService, TextToSpeechProviderService>();
                services.AddSingleton<ISsmlConverterService, SsmlConverterService>();
                services.AddSingleton<IProjectInitializer, FileProjectInitializer>();
                services.AddTransient<ProjectReader>();
                services.AddTransient<ProjectWriter>();

                services.AddSingleton<RecentlyUsedItemsProjectWatcher>();
                services.AddSingleton<MainWindowTitleProjectWatcher>();
                services.AddSingleton<ProjectManagementCloseApplicationWatcher>();
                services.AddTransient<WindowCommandsView>();

                services.AddLogging(x =>
                {
                    x.AddConsole();
                    x.AddDebug();
                });
            });

        _host = hostBuilder.Build();

        IoCContainer.ServiceProvider = _host.Services;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var serviceProvider = IoCContainer.ServiceProvider;

        var configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
        await configurationService.LoadAsync();

        serviceProvider.CreateTypesThatMustBeConstructedAtStartup();

        var languageService = serviceProvider.GetRequiredService<ILanguageService>();

        // Note: it's best to use .CurrentUICulture in actual apps since it will use the preferred language
        // of the user. But in order to demo multilingual features for devs (who mostly have en-US as .CurrentUICulture),
        // we use .CurrentCulture for the sake of the demo
        languageService.PreferredCulture = CultureInfo.CurrentCulture;
        languageService.FallbackCulture = new CultureInfo("en-US");

        var shellService = serviceProvider.GetRequiredService<IShellService>();
        await shellService.CreateAsync<ShellWindow>();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync();
        }

        base.OnExit(e);
    }
}
