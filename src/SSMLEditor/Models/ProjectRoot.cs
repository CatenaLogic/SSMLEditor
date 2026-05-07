namespace SSMLEditor;

using System.Collections.Generic;

public class ProjectRoot
{
    public ProjectRoot()
    {
        Languages = new List<Language>();
        Video = new Video();
    }

    public List<Language> Languages { get; init; }

    public Video Video { get; init; } 
}
