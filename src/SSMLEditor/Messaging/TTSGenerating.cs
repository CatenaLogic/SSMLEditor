namespace SSMLEditor.Messaging
{
    using Catel.Messaging;

    public class TTSGenerating : MessageBase<TTSGenerating, Language>
    {
        public TTSGenerating()
        {
        }

        public TTSGenerating(Language language)
            : base(language)
        {
        }
    }
}
