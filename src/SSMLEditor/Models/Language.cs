namespace SSMLEditor
{
    using System.Globalization;
    using System.IO;
    using Catel.Data;
    using Newtonsoft.Json;

    public class Language : ObservableObject
    {
        private string _outputRelativeFileName;

        public string RelativeFileName { get; set; }

        public string OutputRelativeFileName
        {
            get
            {
                if (string.IsNullOrEmpty(_outputRelativeFileName))
                {
                    var relativeFileName = RelativeFileName;
                    if (string.IsNullOrWhiteSpace(relativeFileName))
                    {
                        return string.Empty;
                    }

                    return Path.ChangeExtension(relativeFileName, ".wav");
                }

                return _outputRelativeFileName;
            }
            set => _outputRelativeFileName = value;
        }

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
