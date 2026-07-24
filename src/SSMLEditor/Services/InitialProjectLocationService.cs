namespace SSMLEditor.Services;

using System;
using System.Threading.Tasks;

public class InitialProjectLocationService : Orc.ProjectManagement.IInitialProjectLocationService
{

    public InitialProjectLocationService()
    {

    }

    public Task<string?> GetInitialProjectLocationAsync()
    {
        return Task.FromResult<string?>(string.Empty);
    }
}
