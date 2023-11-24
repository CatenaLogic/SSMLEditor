namespace SSMLEditor.ProjectManagement
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Catel.Logging;
    using Newtonsoft.Json;
    using Orc.FileSystem;
    using Orc.ProjectManagement;

    public class ProjectWriter : ProjectWriterBase<Project>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;

        public ProjectWriter(IFileService fileService, IDirectoryService directoryService)
        {
            ArgumentNullException.ThrowIfNull(fileService);
            ArgumentNullException.ThrowIfNull(directoryService);

            _fileService = fileService;
            _directoryService = directoryService;
        }

        protected override async Task<bool> WriteToLocationAsync(Project project, string location)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(project.ProjectRoot, jsonSettings);

            await _fileService.WriteAllTextAsync(location, json);

            var directory = Path.GetDirectoryName(location);

            foreach (var language in project.ProjectRoot.Languages)
            {
                Log.Debug($"Saving project language '{language}'");

                var languageFileName = Path.Combine(directory, language.RelativeFileName);
                var languageDirectory = Path.GetDirectoryName(languageFileName);

                _directoryService.Create(languageDirectory);

                await _fileService.WriteAllTextAsync(languageFileName, language.Content);

                language.OriginalContent = language.Content;
            }

            return true;
        }
    }
}
