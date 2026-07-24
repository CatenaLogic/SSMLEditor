namespace SSMLEditor.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Catel.Services;
using Orc.FileSystem;
using Orc.Serialization.Json;
using SSMLEditor.Providers;

public class TextToSpeechProviderService : ITextToSpeechProviderService
{
    private readonly IFileService _fileService;
    private readonly IAppDataService _appDataService;
    private readonly IJsonSerializerFactory _jsonSerializerFactory;
    private readonly IJsonSerializer _serializer;

    private readonly IReadOnlyList<ITextToSpeechProvider> _availableProviders;

    public TextToSpeechProviderService(IEnumerable<ITextToSpeechProvider> availableProviders,
        IFileService fileService, IAppDataService appDataService, IJsonSerializerFactory jsonSerializerFactory)
    {
        _availableProviders = availableProviders.ToArray();
        _fileService = fileService;
        _appDataService = appDataService;
        _jsonSerializerFactory = jsonSerializerFactory;

        var serializerSettings = new JsonSerializerSettings
        {
            
        };

        // Don't expose full type names, so use a custom type info resolver
        var resolver = new DefaultJsonTypeInfoResolver();
        resolver.Modifiers.Add(x =>
        {
            if (x.Type != typeof(ITextToSpeechProvider))
            {
                return;
            }

            if (x.Kind == JsonTypeInfoKind.None)
            {
                return;
            }

            x.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                IgnoreUnrecognizedTypeDiscriminators = false,
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(AzureCognitiveServices), "AzureCognitiveServices")
                }
            };
        });

        serializerSettings.TypeInfoResolverChain.Add(resolver);

        _serializer = _jsonSerializerFactory.CreateSerializer(serializerSettings);

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

            var deserialized = _serializer.DeserializeFromString<ITextToSpeechProvider[]>(json);
            if (deserialized is not null)
            {
                providers.AddRange(deserialized);
            }
        }

        providers.ForEach(x => x.RemoveDuplicateProperties());

        Providers = providers;
    }

    public async Task SaveAsync()
    {
        var serializer = _jsonSerializerFactory.CreateSerializer();

        var providers = Providers.ToList();

        providers.ForEach(x => x.RemoveDuplicateProperties());

        var json = _serializer.SerializeToString(providers);
        var filename = GetFilename();

        await _fileService.WriteAllTextAsync(filename, json);
    }

    protected string GetFilename()
    {
        var directory = _appDataService.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming);
        return Path.Combine(directory, "providers.json");
    }
}
