namespace SSMLEditor
{
    using System;
    using System.IO;
    using Catel;

    public static class ProjectExtensions
    {
        public static string GetFullPath(this Project project, Language language)
        {
            ArgumentNullException.ThrowIfNull(project);
            ArgumentNullException.ThrowIfNull(language);

            return project.GetFullPath(language.RelativeFileName);
        }

        public static string GetFullAudioPath(this Project project, Language language)
        {
            ArgumentNullException.ThrowIfNull(project);
            ArgumentNullException.ThrowIfNull(language);

            var fileName = project.GetFullPath(language.OutputRelativeFileName);
            return fileName;
        }

        public static string GetFullPath(this Project project, Video video)
        {
            ArgumentNullException.ThrowIfNull(project);
            ArgumentNullException.ThrowIfNull(video);

            return project.GetFullPath(video.RelativeFileName);
        }

        public static string GetFullPath(this Project project, string relativeFileName)
        {
            ArgumentNullException.ThrowIfNull(project);
            Argument.IsNotNullOrWhitespace(() => relativeFileName);

            var directory = Path.GetDirectoryName(project.Location);
            var fileName = Path.Combine(directory, relativeFileName);

            fileName = fileName.Replace("/", "\\");

            return fileName;
        }
    }
}
