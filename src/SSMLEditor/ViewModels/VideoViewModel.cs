namespace SSMLEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.Messaging;
    using Catel.MVVM;
    using Orc.FileSystem;
    using Orc.ProjectManagement;
    using Orc.SelectionManagement;
    using SSMLEditor.Messaging;
    using SSMLEditor.Providers;

    public class VideoViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IProjectManager _projectManager;
        private readonly ISelectionManager<Language> _languageSelectionManager;
        private readonly IFileService _fileService;
        private readonly IMessageMediator _messageMediator;

        public VideoViewModel(IProjectManager projectManager, ISelectionManager<Language> languageSelectionManager,
            IFileService fileService, IMessageMediator messageMediator)
        {
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => languageSelectionManager);
            Argument.IsNotNull(() => fileService);
            Argument.IsNotNull(() => messageMediator);

            _projectManager = projectManager;
            _languageSelectionManager = languageSelectionManager;
            _fileService = fileService;
            _messageMediator = messageMediator;

            Play = new TaskCommand(OnPlayExecuteAsync, OnPlayCanExecute);
            Pause = new TaskCommand(OnPauseExecuteAsync, OnPauseCanExecute);
        }

        public override string Title { get { return "Video"; } }

        public Uri VideoUri { get; private set; }

        public Uri AudioUri { get; private set; }

        public Uri BaseAudioUri { get; private set; }

        public TimeSpan Position { get; set; }

        public TimeSpan TotalDuration { get; set; }

        public bool IsPlaying { get; private set; }

        public bool EnableBaseAudioTrack { get; set; }

        public bool CanSelectedBaseAudioTrack { get; set; }

        public List<Language> AvailableBaseAudioTracks { get; private set; }

        public Language SelectedBaseAudioTrack { get; set; }

        #region Commands
        public TaskCommand Play { get; private set; }

        private bool OnPlayCanExecute()
        {
            if (VideoUri is null)
            {
                return false;
            }

            if (AudioUri is null)
            {
                return false;
            }

            if (IsPlaying)
            {
                return false;
            }

            return true;
        }

        private async Task OnPlayExecuteAsync()
        {
            IsPlaying = true;
        }

        public TaskCommand Pause { get; private set; }

        private bool OnPauseCanExecute()
        {
            if (VideoUri is null)
            {
                return false;
            }

            if (!IsPlaying)
            {
                return false;
            }

            return true;
        }

        private async Task OnPauseExecuteAsync()
        {
            IsPlaying = false;
        }

        #endregion

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _projectManager.ProjectActivatedAsync += OnProjectManagerProjectActivedAsync;
            _projectManager.ProjectClosedAsync += OnProjectManagerProjectClosedAsync;
            _languageSelectionManager.SelectionChanged += OnLanguageSelectionManagerSelectionChanged;
            _messageMediator.Register<TTSGenerating>(this, OnTTSGenerating);
            _messageMediator.Register<TTSGenerated>(this, OnTTSGenerated);

            UpdateProject();
        }

        protected override async Task CloseAsync()
        {
            _projectManager.ProjectActivatedAsync -= OnProjectManagerProjectActivedAsync;
            _projectManager.ProjectClosedAsync -= OnProjectManagerProjectClosedAsync;
            _languageSelectionManager.SelectionChanged -= OnLanguageSelectionManagerSelectionChanged;
            _messageMediator.Unregister<TTSGenerating>(this, OnTTSGenerating);
            _messageMediator.Unregister<TTSGenerated>(this, OnTTSGenerated);

            await base.CloseAsync();
        }

        private async Task OnProjectManagerProjectActivedAsync(object sender, ProjectUpdatedEventArgs e)
        {
            if (e.NewProject is null)
            {
                // Don't support for now
                return;
            }

            UpdateProject();
        }

        private async Task OnProjectManagerProjectClosedAsync(object sender, ProjectEventArgs e)
        {
            // Don't support for now
            //UpdateProject();
        }

        private void OnLanguageSelectionManagerSelectionChanged(object sender, SelectionChangedEventArgs<Language> e)
        {
            Pause.Execute();

            UpdateProject();
        }

        private void OnSelectedBaseAudioTrackChanged()
        {
            UpdateBaseAudio();
        }

        private void OnEnableBaseAudioTrackChanged()
        {
            UpdateBaseAudio();
        }

        private void OnTTSGenerating(TTSGenerating message)
        {
            if (message.Data == _languageSelectionManager.GetSelectedItem())
            {
                Log.Info($"Current language is being (re)generated, stopping playback");

                Pause.Execute();
                AudioUri = null;
            }
        }

        private void OnTTSGenerated(TTSGenerated message)
        {
            if (message.Data == _languageSelectionManager.GetSelectedItem())
            {
                Log.Info($"Current language is (re)generated");

                UpdateProject();
            }
        }

        private void UpdateBaseAudio()
        {
            Uri baseAudioUri = null;

            var availableBaseAudioTracks = new List<Language>();

            var project = _projectManager.GetActiveProject<Project>();
            if (project is not null)
            {
                availableBaseAudioTracks.AddRange(from x in project.ProjectRoot.Languages
                                                  orderby x.Culture.DisplayName
                                                  select x);

                var selectedBaseAudioTrack = SelectedBaseAudioTrack ?? availableBaseAudioTracks.FirstOrDefault();
                if (selectedBaseAudioTrack is not null)
                {
                    var audioFileName = project.GetFullAudioPath(selectedBaseAudioTrack);

                    if (_fileService.Exists(audioFileName))
                    {
                        baseAudioUri = new Uri(audioFileName, UriKind.RelativeOrAbsolute);
                    }
                }
            }

            AvailableBaseAudioTracks = availableBaseAudioTracks;
            CanSelectedBaseAudioTrack = AvailableBaseAudioTracks.Count > 0;

            if (SelectedBaseAudioTrack is null)
            {
                SelectedBaseAudioTrack = availableBaseAudioTracks.FirstOrDefault();
            }

            if (!EnableBaseAudioTrack)
            {
                baseAudioUri = null;
            }

            BaseAudioUri = baseAudioUri;
        }

        private void UpdateProject()
        {
            if (IsClosing || IsClosed)
            {
                return;
            }

            Uri videoUri = null;
            Uri audioUri = null;

            var project = _projectManager.GetActiveProject<Project>();
            if (project is not null)
            {
                var videoFileName = project.GetFullPath(project.ProjectRoot.Video);

                if (_fileService.Exists(videoFileName))
                {
                    videoUri = new Uri(videoFileName, UriKind.RelativeOrAbsolute);
                }

                var language = _languageSelectionManager.GetSelectedItem();
                if (language is not null)
                {
                    var audioFileName = project.GetFullAudioPath(language);

                    if (_fileService.Exists(audioFileName))
                    {
                        audioUri = new Uri(audioFileName, UriKind.RelativeOrAbsolute);
                    }
                }
            }

            VideoUri = videoUri;
            AudioUri = audioUri;

            UpdateBaseAudio();

            if (IsPlaying && videoUri is null)
            {
                IsPlaying = false;
            }
        }
    }
}
