namespace SSMLEditor;

using System;
using Orc.ProjectManagement;

public sealed class Project : ProjectBase, IProject, IEquatable<Project>
{
    public Project(string location)
        : this(location, location)
    {
        // Keep empty
    }

    public Project(string location, string title)
        : base(location, title)
    {
        ProjectRoot = new ProjectRoot();
    }

    public ProjectRoot ProjectRoot { get; init; }

    public bool Equals(Project? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Location, other.Location);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((Project) obj);
    }

    public override int GetHashCode()
    {
        return (Location is not null ? Location.GetHashCode() : 0);
    }

    public void SetIsDirty(bool isDirty)
    {
        if (isDirty)
        {
            MarkAsDirty();
        }
        else
        {
            ClearIsDirty();
        }
    }
}
