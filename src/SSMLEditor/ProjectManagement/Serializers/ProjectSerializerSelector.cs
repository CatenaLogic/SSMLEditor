namespace SSMLEditor.ProjectManagement;

using System;
using Microsoft.Extensions.DependencyInjection;
using Orc.ProjectManagement;

internal class ProjectSerializerSelector : IProjectSerializerSelector
{
    private readonly IServiceProvider _serviceProvider;

    public ProjectSerializerSelector(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    public IProjectReader GetReader(string location)
    {
        return _serviceProvider.GetRequiredService<ProjectReader>();
    }

    public IProjectWriter GetWriter(string location)
    {
        return _serviceProvider.GetRequiredService<ProjectWriter>();
    }
}
