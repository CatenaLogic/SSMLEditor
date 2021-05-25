namespace SSMLEditor
{
    using System.IO;
    using Catel;

    public static class ProjectExtensions
    {
        public static string GetFullPath(this Project project, Language language)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNull(() => language);

            return project.GetFullPath(language.RelativeFileName);
        }

        public static string GetFullAudioPath(this Project project, Language language)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNull(() => language);

            var fileName = project.GetFullPath(language.RelativeFileName);

            fileName = Path.ChangeExtension(fileName, ".wav");

            return fileName;
        }

        public static string GetFullPath(this Project project, Video video)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNull(() => video);

            return project.GetFullPath(video.RelativeFileName);
        }

        public static string GetFullPath(this Project project, string relativeFileName)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNullOrWhitespace(() => relativeFileName);

            var directory = Path.GetDirectoryName(project.Location);
            var fileName = Path.Combine(directory, relativeFileName);

            fileName = fileName.Replace("/", "\\");

            return fileName;
        }
    }
}
