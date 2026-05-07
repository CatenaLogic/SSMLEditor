namespace SSMLEditor.ProjectManagement;

using System;
using System.IO;
using System.Threading.Tasks;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using Orc.FileSystem;
using Orc.ProjectManagement;
using Orc.Serialization.Json;

public class ProjectWriter : ProjectWriterBase<Project>
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ProjectWriter));

    private readonly IFileService _fileService;
    private readonly IDirectoryService _directoryService;
    private readonly IJsonSerializerFactory _jsonSerializerFactory;

    public ProjectWriter(IFileService fileService, IDirectoryService directoryService,
        IJsonSerializerFactory jsonSerializerFactory)
    {
        ArgumentNullException.ThrowIfNull(fileService);
        ArgumentNullException.ThrowIfNull(directoryService);

        _fileService = fileService;
        _directoryService = directoryService;
        _jsonSerializerFactory = jsonSerializerFactory;
    }

    protected override async Task<bool> WriteToLocationAsync(Project project, string location)
    {
        var serializer = _jsonSerializerFactory.CreateSerializer();

        var json = serializer.SerializeToString(project.ProjectRoot);

        await _fileService.WriteAllTextAsync(location, json);

        var directory = Path.GetDirectoryName(location);

        foreach (var language in project.ProjectRoot.Languages)
        {
            Logger.LogDebug("Saving project language '{Language}'", language);

            var languageFileName = Path.Combine(directory, language.RelativeFileName);
            var languageDirectory = Path.GetDirectoryName(languageFileName);

            _directoryService.Create(languageDirectory);

            await _fileService.WriteAllTextAsync(languageFileName, language.Content);

            language.OriginalContent = language.Content;
        }

        return true;
    }
}
