namespace SSMLEditor.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Catel.Services;
using Newtonsoft.Json;
using Orc.FileSystem;
using SSMLEditor.Providers;
using SSMLEditor.Serialization;

public class TextToSpeechProviderService : ITextToSpeechProviderService
{
    private readonly IFileService _fileService;
    private readonly IAppDataService _appDataService;
    private readonly IReadOnlyList<ITextToSpeechProvider> _availableProviders;

    public TextToSpeechProviderService(IEnumerable<ITextToSpeechProvider> availableProviders, IFileService fileService, IAppDataService appDataService)
    {
        ArgumentNullException.ThrowIfNull(availableProviders);
        ArgumentNullException.ThrowIfNull(fileService);
        ArgumentNullException.ThrowIfNull(appDataService);

        _availableProviders = availableProviders.ToArray();
        _fileService = fileService;
        _appDataService = appDataService;

        Providers = new List<ITextToSpeechProvider>();
    }

    public IEnumerable<ITextToSpeechProvider> GetAvailableProviders()
    {
        return _availableProviders;
    }

    public List<ITextToSpeechProvider> Providers { get; private set; }

    public async Task LoadAsync()
    {
        var providers = new List<ITextToSpeechProvider>();

        var filename = GetFilename();
        if (_fileService.Exists(filename))
        {
            var json = await _fileService.ReadAllTextAsync(filename);

            JsonConvert.PopulateObject(json, providers, GetSettings());
        }

        providers.ForEach(x => x.RemoveDuplicateProperties());

        Providers = providers;
    }

    public async Task SaveAsync()
    {
        var providers = Providers.ToList();

        providers.ForEach(x => x.RemoveDuplicateProperties());

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(providers, GetSettings());
        var filename = GetFilename();

        await _fileService.WriteAllTextAsync(filename, json);
    }

    protected JsonSerializerSettings GetSettings()
    {
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new SafetySerializationBinder(),
        };

        return settings;
    }

    protected string GetFilename()
    {
        var directory = _appDataService.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming);
        return Path.Combine(directory, "providers.json");
    }
}
