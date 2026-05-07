namespace SSMLEditor.ProjectManagement;

using System;
using System.IO;
using System.Threading.Tasks;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orc.FileSystem;
using Orc.Notifications;
using Orc.ProjectManagement;

public class ProjectReader : ProjectReaderBase
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ProjectReader));

    private readonly IFileService _fileService;
    private readonly INotificationService _notificationService;

    public ProjectReader(IFileService fileService, INotificationService notificationService)
    {
        ArgumentNullException.ThrowIfNull(fileService);
        ArgumentNullException.ThrowIfNull(notificationService);

        _fileService = fileService;
        _notificationService = notificationService;
    }

    protected override async Task<IProject> ReadFromLocationAsync(string location)
    {
        try
        {
            var project = new Project(location)
            {
            };

            var json = await _fileService.ReadAllTextAsync(location);

            var jsonSettings = new JsonSerializerSettings
            {

            };

            JsonConvert.PopulateObject(json, project.ProjectRoot, jsonSettings);

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
