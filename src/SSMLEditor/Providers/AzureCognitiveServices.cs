namespace SSMLEditor.Providers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using MethodTimer;
    using Microsoft.CognitiveServices.Speech;

    public class AzureCognitiveServices : TextToSpeechProviderBase
    {
        public AzureCognitiveServices()
        {
            Properties.Add(new TtsProperty
            {
                Name = "SubscriptionKey",
                DisplayName = "Subscription key",
                Description = "The subscription key for Azure"
            });

            Properties.Add(new TtsProperty
            {
                Name = "ServiceRegion",
                DisplayName = "Service region",
                Description = "The service region for Azure"
            });

#if DEBUG
            SubscriptionKey = "";
            ServiceRegion = "";
#endif
        }

        public override string Name => "Azure Cognitive Services";

        public string SubscriptionKey
        {
            get { return this["SubscriptionKey"].Value; }
            set { this["SubscriptionKey"].Value = value; }
        }

        public string ServiceRegion
        {
            get { return this["ServiceRegion"].Value; }
            set { this["ServiceRegion"].Value = value; }
        }

        public override async Task<IEnumerable<TtsLanguage>> GetLanguagesAsync()
        {
            var languages = new List<TtsLanguage>();

            // Fixed, seems there is no api for this

            languages.Add(new TtsLanguage
            {
                Name = "English (US)",
                CultureInfo = new CultureInfo("en-US")
            });

            languages.Add(new TtsLanguage
            {
                Name = "Nederlands (NL)",
                CultureInfo = new CultureInfo("nl-NL")
            });

            return languages;
        }

        public override async Task<IEnumerable<TtsVoice>> GetVoicesAsync(TtsLanguage language)
        {
            var voices = new List<TtsVoice>();

            var config = SpeechConfig.FromSubscription(SubscriptionKey, ServiceRegion);

            using (var synthesizer = new SpeechSynthesizer(config, null))
            {
                using (var azureVoices = await synthesizer.GetVoicesAsync(language.CultureInfo.TwoLetterISOLanguageName))
                {
                    foreach (var azureVoice in azureVoices.Voices)
                    {
                        var voice = new TtsVoice
                        {
                            Id = azureVoice.Name,
                            Name = azureVoice.Name,
                            ShortName = azureVoice.ShortName,
                            LocalName = azureVoice.LocalName,
                            Language = language.CultureInfo,
                            IsNeural = azureVoice.VoiceType == SynthesisVoiceType.OnlineNeural,
                        };

                        switch (azureVoice.Gender)
                        {
                            case SynthesisVoiceGender.Female:
                                voice.Gender = TtsGender.Female;
                                break;

                            case SynthesisVoiceGender.Male:
                                voice.Gender = TtsGender.Male;
                                break;

                            case SynthesisVoiceGender.Unknown:
                                voice.Gender = TtsGender.Unknown;
                                break;
                        }

                        voices.Add(voice);
                    }
                }
            }

            //voices.Add(new Voice
            //{
            //    Id = "en-US-JennyNeural",
            //    Name = "Jenny (Neural)",
            //    Language = new CultureInfo("en-US"),
            //    IsNeural = true,
            //});

            //voices.Add(new Voice
            //{
            //    Id = "en-US-GuyNeural",
            //    Name = "Guy (Neural)",
            //    Language = new CultureInfo("en-US"),
            //    IsNeural = true,
            //});

            //voices.Add(new Voice
            //{
            //    Id = "nl-NL-ColetteNeural",
            //    Name = "Colette (Neural)",
            //    Language = new CultureInfo("nl-NL"),
            //    IsNeural = true,
            //});

            //voices.Add(new Voice
            //{
            //    Id = "nl-NL-MaartenNeural",
            //    Name = "Maarten (Neural)",
            //    Language = new CultureInfo("nl-NL"),
            //    IsNeural = true,
            //});

            // TODO: Add voices

            return voices;
        }

        [Time]
        public override async Task<Stream> ExecuteAsync(string ssml)
        {
            var config = SpeechConfig.FromSubscription(SubscriptionKey, ServiceRegion);

            using (var synthesizer = new SpeechSynthesizer(config, null))
            {
                var result = await synthesizer.SpeakSsmlAsync(ssml);

                var memoryStream = new MemoryStream();

                await memoryStream.WriteAsync(result.AudioData);

                return memoryStream;
            }
        }
    }
}
