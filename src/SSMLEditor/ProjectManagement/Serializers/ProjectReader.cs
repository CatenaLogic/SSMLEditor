namespace SSMLEditor.ProjectManagement;

using System;
using System.IO;
using System.Threading.Tasks;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using Orc.FileSystem;
using Orc.Notifications;
using Orc.ProjectManagement;
using Orc.Serialization.Json;

public class ProjectReader : ProjectReaderBase
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ProjectReader));

    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;
    private readonly IJsonSerializerFactory _jsonSerializerFactory;

    private readonly IJsonSerializer _jsonSerializer;

    public ProjectReader(IFileService fileService, INotificationService notificationService,
        IJsonSerializerFactory jsonSerializerFactory)
    {
        _fileService = fileService;
        _notificationService = notificationService;
        _jsonSerializerFactory = jsonSerializerFactory;

        _jsonSerializer = _jsonSerializerFactory.CreateSerializer(new JsonSerializerSettings
        {
            PropertyNameCaseInsensitive = true
        });
    }

    protected override async Task<IProject> ReadFromLocationAsync(string location)
    {
        try
        {
            var json = await _fileService.ReadAllTextAsync(location);

            var projectRoot = _jsonSerializer.DeserializeFromString<ProjectRoot>(json);

            var project = new Project(location)
            {
                ProjectRoot = projectRoot
            };

            var directory = Path.GetDirectoryName(location);

            foreach (var language in project.ProjectRoot.Languages)
            {
                Logger.LogDebug("Reading project language '{Language}'", language);

                var languageFileName = project.GetFullPath(language);

                if (!_fileService.Exists(languageFileName))
                {
                    Logger.LogWarning("Could not find '{LanguageFileName}'", languageFileName);
                    continue;
                }

                language.Content = await _fileService.ReadAllTextAsync(languageFileName);
                language.OriginalContent = language.Content;
            }

            return project;

        }
        catch (System.IO.IOException ex)
        {
            _notificationService.ShowNotification("Could not open file", ex.Message);
        }

        return null;
    }
}
