﻿namespace SSMLEditor
{
    using System.Threading.Tasks;
    using Catel.MVVM;

    public class HelpAboutCommandContainer : CommandContainerBase
    {
        public HelpAboutCommandContainer(ICommandManager commandManager)
            : base(Commands.Help.About, commandManager)
        {
        }

        protected override Task ExecuteAsync(object parameter)
        {
            return base.ExecuteAsync(parameter);
        }
    }
}
