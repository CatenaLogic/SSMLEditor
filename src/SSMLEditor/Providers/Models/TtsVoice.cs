namespace SSMLEditor.Providers
{
    using System.Globalization;

    public class TtsVoice
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string LocalName { get; set; }

        public CultureInfo Language { get; set; }

        public TtsGender Gender { get; set; }

        public bool IsNeural { get; set; }

        public override string ToString()
        {
            return $"{Language.TwoLetterISOLanguageName} - {Name}";
        }
    }
}
