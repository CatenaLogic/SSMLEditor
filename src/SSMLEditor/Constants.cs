namespace SSMLEditor
{
    using System.Collections.Immutable;
    using System.Windows.Input;
    using Orc.Squirrel;
    using InputGesture = Catel.Windows.Input.InputGesture;

    public static class Settings
    {
        public static class Application
        {
            public static class General
            {
                public const string ThemeBaseColor = "General.ThemeBaseColor";
                public const string ThemeBaseColorDefaultValue = "Light";
            }

            public static class AutomaticUpdates
            {
                public const bool CheckForUpdatesDefaultValue = true;

                public static readonly ImmutableArray<UpdateChannel> AvailableChannels = ImmutableArray.Create(
                    new UpdateChannel("Stable", "https://downloads.catenalogic.com/ssmleditor/stable"),
                    new UpdateChannel("Beta", "https://downloads.catenalogic.com/ssmleditor/beta"),
                    new UpdateChannel("Alpha", "https://downloads.catenalogic.com/ssmleditor/alpha")
                );

                public static readonly UpdateChannel DefaultChannel = AvailableChannels[0];
            }
        }
    }

    public static class Commands
    {
        public static class Help
        {
            public const string About = "Help.About";
            public static readonly InputGesture AboutInputGesture = null;
        }

        public static class Settings
        {
            public const string General = "Settings.General";
            public static readonly InputGesture GeneralInputGesture = new InputGesture(Key.S, ModifierKeys.Alt | ModifierKeys.Control);
        }

        public static class TTS
        {
            public const string Generate = "TTS.Generate";
            public static readonly InputGesture CloseInputGesture = null;

            public const string GenerateAll = "TTS.GenerateAll";
            public static readonly InputGesture OpenInputGesture = null;
        }

        public static class Project
        {
            public const string Close = "Project.Close";
            public static readonly InputGesture CloseInputGesture = new InputGesture(Key.X, ModifierKeys.Alt);

            public const string Open = "Project.Open";
            public static readonly InputGesture OpenInputGesture = new InputGesture(Key.O, ModifierKeys.Control);

            public const string Save = "Project.Save";
            public static readonly InputGesture SaveInputGesture = new InputGesture(Key.S, ModifierKeys.Control);

            //public const string SaveAs = "Project.SaveAs";
            //public static readonly InputGesture SaveAsInputGesture = null;
        }
    }
}
