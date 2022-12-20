namespace SSMLEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;
    using Orc.ProjectManagement;
    using Orc.SelectionManagement;

    public class EditorsViewModel : ViewModelBase
    {
        private readonly ISelectionManager<Language> _languageSelectionManager;
        private readonly IProjectManager _projectManager;

        public EditorsViewModel(ISelectionManager<Language> languageSelectionManager, IProjectManager projectManager)
        {
            ArgumentNullException.ThrowIfNull(languageSelectionManager);
            ArgumentNullException.ThrowIfNull(projectManager);

            _languageSelectionManager = languageSelectionManager;
            _projectManager = projectManager;
        }

        public List<Language> Languages { get; private set; }

        public Language SelectedLanguage { get; set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _languageSelectionManager.SelectionChanged += OnLanguageSelectionManagerSelectionChanged;
            _projectManager.ProjectActivatedAsync += OnProjectActivatedAsync;
            _projectManager.ProjectClosedAsync += OnProjectClosedAsync;

            UpdateTabs();
        }

        protected override async Task CloseAsync()
        {
            _languageSelectionManager.SelectionChanged -= OnLanguageSelectionManagerSelectionChanged;
            _projectManager.ProjectActivatedAsync -= OnProjectActivatedAsync;
            _projectManager.ProjectClosedAsync -= OnProjectClosedAsync;

            await base.CloseAsync();
        }

        private void OnLanguageSelectionManagerSelectionChanged(object sender, SelectionChangedEventArgs<Language> e)
        {
            UpdateTabs();
        }

        private async Task OnProjectActivatedAsync(object sender, ProjectUpdatedEventArgs e)
        {
            UpdateTabs();
        }

        private async Task OnProjectClosedAsync(object sender, ProjectEventArgs e)
        {
            UpdateTabs();
        }

        private void OnSelectedLanguageChanged()
        {
            _languageSelectionManager.Replace(SelectedLanguage);
        }

        private void UpdateTabs()
        {
            var languages = new List<Language>();

            var project = _projectManager.GetActiveProject<Project>();
            if (project is not null)
            {
                languages.AddRange(project.ProjectRoot.Languages.OrderBy(x => x.Culture.TwoLetterISOLanguageName));
            }

            Languages = languages;
            SelectedLanguage = _languageSelectionManager.GetSelectedItem() ?? Languages.FirstOrDefault();
        }
    }
}
