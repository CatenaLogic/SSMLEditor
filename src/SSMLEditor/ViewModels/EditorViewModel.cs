namespace SSMLEditor.ViewModels
{
    using Catel;
    using Catel.MVVM;

    public class EditorViewModel : ViewModelBase
    {
        public EditorViewModel(Language language)
        {
            Argument.IsNotNull(() => language);

            Language = language;
        }

        public Language Language { get; private set; }

        public string Content { get; set; }
    }
}
