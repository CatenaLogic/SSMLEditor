namespace SSMLEditor
{
    using System.Globalization;
    using Catel.Data;
    using Newtonsoft.Json;

    public class Language : ObservableObject
    {
        public string RelativeFileName { get; set; }

        public CultureInfo Culture { get; set; }

        [JsonIgnore]
        public bool IsDirty
        {
            get
            {
                var equal = string.Equals(Content, OriginalContent);
                return !equal;
            }
        }

        [JsonIgnore]
        public string ShortName
        {
            get { return Culture.TwoLetterISOLanguageName; }
        }

        [JsonIgnore]
        public string Content { get; set; }

        [JsonIgnore]
        public string OriginalContent { get; set; }

        [JsonIgnore]
        public string Status
        {
            get
            {
                var text = $"{ShortName}";

                if (IsDirty)
                {
                    text += " *";
                }

                return text;
            }
        }

        public override string ToString()
        {
            return $"{RelativeFileName} | {Culture?.TwoLetterISOLanguageName}";
        }
    }
}
