namespace SSMLEditor.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Documents;
    using Catel;
    using Catel.MVVM;
    using Orc.ProjectManagement;
    using SSMLEditor.Services;

    public class EditorViewModel : ViewModelBase
    {
        private readonly IProjectManager _projectManager;
        private readonly ISsmlConverterService _ssmlConverterService;

        private bool _isUpdating;

        public EditorViewModel(Language language, IProjectManager projectManager, ISsmlConverterService ssmlConverterService)
        {
            Argument.IsNotNull(() => language);
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => ssmlConverterService);

            Language = language;
            _projectManager = projectManager;
            _ssmlConverterService = ssmlConverterService;
        }

        public Language Language { get; private set; }

        public FlowDocument RichDocument { get; private set; }

        public FlowDocument SsmlDocument { get; private set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _projectManager.ProjectSavingAsync += OnProjectManagerSavingAsync;

            UpdateSsmlDocument();
            UpdateRichDocument();
        }

        protected override async Task CloseAsync()
        {
            _projectManager.ProjectSavingAsync -= OnProjectManagerSavingAsync;

            await base.CloseAsync();
        }

        private async Task OnProjectManagerSavingAsync(object sender, ProjectCancelEventArgs e)
        {
            //UpdateLanguage();
        }

        public void MarkRichDocumentAsChanged()
        {
            // TODO: Update ssml based on rich document
        }

        public void MarkSsmlDocumentAsChanged()
        {
            var document = SsmlDocument;
            if (document is not null)
            {
                Language.Content = new TextRange(document.ContentStart, document.ContentEnd).Text;
            }
        }

        private void UpdateRichDocument()
        {
            if (_isUpdating)
            {
                return;
            }

            using (new DisposableToken<EditorViewModel>(this,
                x => x.Instance._isUpdating = true,
                x => x.Instance._isUpdating = false))
            {
                RichDocument = _ssmlConverterService.ConvertToFlowDocument(Language.Content);
            }
        }

        private void UpdateSsmlDocument()
        {
            if (_isUpdating)
            {
                return;
            }

            using (new DisposableToken<EditorViewModel>(this,
                x => x.Instance._isUpdating = true,
                x => x.Instance._isUpdating = false))
            {
                var doc = new FlowDocument();

                var p = new Paragraph(new Run(Language.Content ?? string.Empty));
                p.FontSize = 16;
                doc.Blocks.Add(p);

                SsmlDocument = doc;
            }
        }
    }
}
