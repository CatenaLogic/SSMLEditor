namespace SSMLEditor.Views
{
    using System.ComponentModel;
    using SSMLEditor.ViewModels;

    public partial class EditorView
    {
        private bool _isUpdatingFromSsmlEditor;

        public EditorView()
        {
            InitializeComponent();
        }

        private void OnRichDocumentTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            (ViewModel as EditorViewModel)?.MarkRichDocumentAsChanged();
        }

        private void OnSsmlDocumentTextChanged(object sender, System.EventArgs e)
        {
            _isUpdatingFromSsmlEditor = true;

            (ViewModel as EditorViewModel)?.MarkSsmlDocumentAsChanged(SsmlTextEditor.Text);

            _isUpdatingFromSsmlEditor = false;
        }

        protected override void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.HasPropertyChanged(nameof(EditorViewModel.SsmlDocument)) &&
                !_isUpdatingFromSsmlEditor)
            {
                SsmlTextEditor.Text = ((EditorViewModel)ViewModel).SsmlDocument;
            }
        }
    }
}
