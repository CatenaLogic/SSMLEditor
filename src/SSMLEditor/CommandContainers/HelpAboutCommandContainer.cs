namespace SSMLEditor;

using System;
using System.Threading.Tasks;
using Catel.MVVM;

public class HelpAboutCommandContainer : CommandContainerBase
{
    public HelpAboutCommandContainer(ICommandManager commandManager, IServiceProvider serviceProvider)
        : base(Commands.Help.About, commandManager, serviceProvider)
    {
    }

    public override Task ExecuteAsync(object? parameter)
    {
        return base.ExecuteAsync(parameter);
    }
}
