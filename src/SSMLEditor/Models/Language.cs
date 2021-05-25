namespace SSMLEditor
{
    using System.Globalization;
    using Newtonsoft.Json;

    public class Language
    {
        public string RelativeFileName { get; set; }

        public CultureInfo Culture { get; set; }

        [JsonIgnore]
        public string ShortName
        {
            get { return Culture.TwoLetterISOLanguageName; }
        }

        [JsonIgnore]
        public string Content { get; set; }

        public override string ToString()
        {
            return $"{RelativeFileName} | {Culture?.TwoLetterISOLanguageName}";
        }
    }
}
