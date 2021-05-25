namespace SSMLEditor.Views
{
    using SSMLEditor.ViewModels;

    public partial class EditorView
    {
        public EditorView()
        {
            InitializeComponent();
        }

        private void OnRichDocumentTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            (ViewModel as EditorViewModel)?.MarkRichDocumentAsChanged();
        }

        private void OnSsmlDocumentTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            (ViewModel as EditorViewModel)?.MarkSsmlDocumentAsChanged();
        }
    }
}
