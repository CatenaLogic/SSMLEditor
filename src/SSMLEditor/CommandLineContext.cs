namespace SSMLEditor
{
    using Orc.CommandLine;

    public class CommandLineContext : ContextBase
    {
        [Option("", "", DisplayName = "initialFile", HelpText = "The initial file to open in SSML Editor")]
        public string InitialFile { get; set; }
    }
}
