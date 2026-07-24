namespace SSMLEditor.ViewModels;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Orc.Wizard;
using SSMLEditor.Providers;
using SSMLEditor.Services;
using SSMLEditor.Wizards.AddProvider;

public class ManageProvidersViewModel : ViewModelBase
{
    private readonly ITextToSpeechProviderService _textToSpeechProviderService;
    private readonly IWizardService _wizardService;
    private readonly IServiceProvider _serviceProvider;

    public ManageProvidersViewModel(IServiceProvider serviceProvider, ITextToSpeechProviderService textToSpeechProviderService,
        IWizardService wizardService)
        : base(serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(textToSpeechProviderService);
        ArgumentNullException.ThrowIfNull(wizardService);

        _serviceProvider = serviceProvider;
        _textToSpeechProviderService = textToSpeechProviderService;
        _wizardService = wizardService;

        Add = new TaskCommand(serviceProvider, OnAddExecuteAsync, OnAddCanExecute);
        Remove = new TaskCommand(serviceProvider, OnRemoveExecuteAsync, OnRemoveCanExecute);
    }

    public List<ITextToSpeechProvider> Providers { get; private set; } = new List<ITextToSpeechProvider>();

    public ITextToSpeechProvider? SelectedProvider { get; set; }

    #region Commands
    public TaskCommand Add { get; private set; }

    private bool OnAddCanExecute()
    {
        return true;
    }

    private async Task OnAddExecuteAsync()
    {
        var wizard = ActivatorUtilities.CreateInstance<AddProviderWizard>(_serviceProvider);
        if ((await _wizardService.ShowWizardAsync(wizard)).DialogResult ?? false)
        {
            var provider = wizard.Provider;
            if (provider is not null)
            {
                Providers.Add(provider);
                RaisePropertyChanged(nameof(Providers));
            }
        }
    }

    public TaskCommand Remove { get; private set; }

    private bool OnRemoveCanExecute()
    {
        return SelectedProvider is not null;
    }

    private async Task OnRemoveExecuteAsync()
    {
        if (SelectedProvider is null)
        {
            return;
        }

        Providers.Remove(SelectedProvider);
        RaisePropertyChanged(nameof(Providers));
        SelectedProvider = null;
    }
    #endregion

    protected override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        Providers = new List<ITextToSpeechProvider>(_textToSpeechProviderService.Providers);
    }

    protected override async Task<bool> SaveAsync()
    {
        _textToSpeechProviderService.Providers.ReplaceRange(Providers);

        await _textToSpeechProviderService.SaveAsync(); 
        
        return await base.SaveAsync();
    }
}
