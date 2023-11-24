namespace SSMLEditor
{
    using System;
    using Orchestra;

    public class ShellActivatedActionQueue : ApplicationWatcherBase
    {
        public void EnqueueAction(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            EnqueueShellActivatedAction(w => action());
        }
    }
}
