namespace SSMLEditor.Providers
{
    using System.Collections.Generic;

    public class TtsContext
    {
        public TtsContext()
        {
            Properties = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Properties { get; private set; }
    }
}
