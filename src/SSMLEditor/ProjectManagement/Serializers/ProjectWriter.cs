namespace SSMLEditor.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Threading;
    using Orc.FileSystem;
    using Orc.ProjectManagement;

    public class ProjectWriter : ProjectWriterBase<Project>
    {
        private readonly IFileService _fileService;

        public ProjectWriter(IFileService fileService)
        {
            Argument.IsNotNull(() => fileService);

            _fileService = fileService;
        }

        protected override Task<bool> WriteToLocationAsync(Project project, string location)
        {
            // TODO: Write all separate files and ssmlx

            //_fileService.WriteAllText(location, project.Text);

            return TaskHelper<bool>.FromResult(true);
        }
    }
}
