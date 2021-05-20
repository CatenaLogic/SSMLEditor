namespace SSMLEditor
{
    using System;
    using Catel;
    using Orchestra;

    public class ShellActivatedActionQueue : ApplicationWatcherBase
    {
        public void EnqueueAction(Action action)
        {
            Argument.IsNotNull(() => action);

            EnqueueShellActivatedAction(w => action());
        }
    }
}
