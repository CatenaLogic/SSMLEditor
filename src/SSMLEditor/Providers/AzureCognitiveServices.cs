namespace SSMLEditor.Providers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Catel.Logging;
using MethodTimer;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;

public class AzureCognitiveServices : TextToSpeechProviderBase
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(AzureCognitiveServices));

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
        SubscriptionKey = string.Empty;
        ServiceRegion = string.Empty;
#endif

        Name = "Azure Cognitive Services";
    }

    [JsonIgnore]
    public string? SubscriptionKey
    {
        get
        {
            var prop = this["SubscriptionKey"];
            return prop?.Value;
        }
        set
        {
            var prop = this["SubscriptionKey"];
            if (prop is not null)
            {
                prop.Value = value;
            }
        }
    }

    [JsonIgnore]
    public string? ServiceRegion
    {
        get
        {
            var prop = this["ServiceRegion"];
            return prop?.Value;
        }
        set
        {
            var prop = this["ServiceRegion"];
            if (prop is not null)
            {
                prop.Value = value;
            }
        }
    }

    public override async Task<IReadOnlyList<TtsLanguage>> GetLanguagesAsync()
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

    public override async Task<IReadOnlyList<TtsVoice>> GetVoicesAsync(TtsLanguage language)
    {
        var voices = new List<TtsVoice>();

        var subscriptionKey = SubscriptionKey;
        var serviceRegion = ServiceRegion;
        if (string.IsNullOrEmpty(subscriptionKey) || string.IsNullOrEmpty(serviceRegion))
        {
            return voices;
        }

        var config = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);

        var cultureInfo = language.CultureInfo;
        if (cultureInfo is null)
        {
            return voices;
        }

        using (var synthesizer = new SpeechSynthesizer(config, null))
        {
            using (var azureVoices = await synthesizer.GetVoicesAsync(cultureInfo.TwoLetterISOLanguageName))
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

        return voices;
    }

    [Time]
    public override async Task<Stream> ExecuteAsync(string ssml)
    {
        var subscriptionKey = SubscriptionKey;
        var serviceRegion = ServiceRegion;
        if (string.IsNullOrEmpty(subscriptionKey) || string.IsNullOrEmpty(serviceRegion))
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("SubscriptionKey and ServiceRegion are required");
        }

        var config = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);

        using (var synthesizer = new SpeechSynthesizer(config, null))
        {
                using (var result = await synthesizer.SpeakSsmlAsync(ssml))
                {
                    if (result.Reason == ResultReason.Canceled)
                    {
                        throw Logger.LogErrorAndCreateException<InvalidOperationException>("Failed to convert text to speech");
                    }

                var memoryStream = new MemoryStream();

                await memoryStream.WriteAsync(result.AudioData);

                return memoryStream;
            }
        }
    }
}
