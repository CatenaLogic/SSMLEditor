namespace SSMLEditor
{
    using System;
    using System.Collections.Generic;
    using Orc.ProjectManagement;

    public class ProjectRoot
    {
        public ProjectRoot()
        {
            Languages = new List<Language>();
            Video = new Video();
        }

        public List<Language> Languages { get; private set; }

        public Video Video { get; private set; } 
    }
}
