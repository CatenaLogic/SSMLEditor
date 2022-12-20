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
            ArgumentNullException.ThrowIfNull(language);
            ArgumentNullException.ThrowIfNull(projectManager);
            ArgumentNullException.ThrowIfNull(ssmlConverterService);

            Language = language;
            _projectManager = projectManager;
            _ssmlConverterService = ssmlConverterService;
        }

        public Language Language { get; private set; }

        public FlowDocument RichDocument { get; private set; }

        public string SsmlDocument { get; set; }

        #region Commands
        
        #endregion

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _projectManager.ProjectSavingAsync += OnProjectManagerSavingAsync;

            UpdateSsmlDocument();
        }

        protected override async Task CloseAsync()
        {
            _projectManager.ProjectSavingAsync -= OnProjectManagerSavingAsync;

            await base.CloseAsync();
        }

        private async Task OnProjectManagerSavingAsync(object sender, ProjectCancelEventArgs e)
        {
            
        }

        public void MarkRichDocumentAsChanged()
        {
            // TODO: Update ssml based on rich document
        }

        public void MarkSsmlDocumentAsChanged(string text)
        {
            Language.Content = text;
            SsmlDocument = text;
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
                SsmlDocument = Language.Content ?? string.Empty;
            }
        }

        private void OnSsmlDocumentChanged()
        {
            UpdateRichDocument();
        }
    }
}
