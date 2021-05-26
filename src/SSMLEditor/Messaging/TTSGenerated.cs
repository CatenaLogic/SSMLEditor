namespace SSMLEditor.Messaging
{
    using Catel.Messaging;

    public class TTSGenerated : MessageBase<TTSGenerated, Language>
    {
        public TTSGenerated()
        {
        }

        public TTSGenerated(Language language)
            : base(language)
        {
        }
    }
}
