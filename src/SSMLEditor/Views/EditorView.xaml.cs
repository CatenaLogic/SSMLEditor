namespace SSMLEditor.Views
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Controls;
    using Catel.MVVM;
    using SSMLEditor.ViewModels;

    public partial class EditorView
    {
        private bool _isUpdatingFromSsmlEditor;

        public EditorView()
        {
            InitializeComponent();
        }

        protected override void OnLoaded(System.EventArgs e)
        {
            base.OnLoaded(e);

            AddEmphasisOptions();
            AddBreakOptions();
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

        private void AddEmphasisOptions()
        {
            var emphasisOptions = new Dictionary<string, string>
            {
                { "None", "none" },
                { "Reduced", "recuded" },
                { "Moderate (default)", "moderate" },
                { "Strong", "strong" },
            };

            foreach (var emphasisOption in emphasisOptions)
            {
                SsmlEmphasisContextMenu.Items.Add(new MenuItem
                {
                    Header = emphasisOption.Key,
                    Command = new Command<EmphasisOption>(AddEmphasis),
                    CommandParameter = new EmphasisOption
                    {
                        Level = emphasisOption.Value,
                    }
                });
            }
        }

        private void AddEmphasis(EmphasisOption emphasisOption)
        {
            var startIndex = SsmlTextEditor.SelectionStart;
            if (startIndex < 0)
            {
                startIndex = SsmlTextEditor.CaretOffset;
                if (startIndex < 0)
                {
                    startIndex = SsmlTextEditor.Document.TextLength - 1;
                }
            }

            var endIndex = startIndex + SsmlTextEditor.SelectionLength;
            if (endIndex < startIndex)
            {
                endIndex = startIndex;
            }

            var startText = $"<emphasis";

            if (!string.IsNullOrWhiteSpace(emphasisOption.Level))
            {
                startText += $" level=\"{emphasisOption.Level}\"";
            }

            startText += ">";

            var endText = "</emphasis>";

            SsmlTextEditor.Document.Insert(endIndex, endText);
            SsmlTextEditor.Document.Insert(startIndex, startText);
        }

        private void AddBreakOptions()
        {
            var timeSpans = new[]
            {
                "250ms",
                "500ms",
                "750ms",
                "1s",
                "1.5s",
                "2s",
                "3s",
                "4s",
                "5s"
            };

            var breakOptions = new Dictionary<string, string>
            {
                { "None", "none" },
                { "Extra weak", "x-weak" },
                { "Weak", "weak" },
                { "Medium (default)", "medium" },
                { "Strong", "strong" },
                { "Extra strong", "x-strong" },
            };

            foreach (var timeSpan in timeSpans)
            {
                var timeSpanMenuItem = new MenuItem
                {
                    Header = timeSpan,
                    Command = new Command<BreakOption>(AddBreak),
                    CommandParameter = new BreakOption
                    {
                        Strength = string.Empty,
                        Time = timeSpan
                    }
                };

                foreach (var breakOption in breakOptions)
                {
                    timeSpanMenuItem.Items.Add(new MenuItem
                    {
                        Header = breakOption.Key,
                        Command = new Command<BreakOption>(AddBreak),
                        CommandParameter = new BreakOption
                        {
                            Strength = breakOption.Value,
                            Time = timeSpan
                        }
                    });
                }

                SsmlBreakContextMenu.Items.Add(timeSpanMenuItem);
            }
        }

        private void AddBreak(BreakOption breakOption)
        {
            var offset = SsmlTextEditor.CaretOffset;
            if (offset < 0)
            {
                offset = SsmlTextEditor.Document.TextLength - 1;
            }

            var text = $"<break";

            if (!string.IsNullOrWhiteSpace(breakOption.Time))
            {
                text += $" time=\"{breakOption.Time}\"";
            }

            if (!string.IsNullOrWhiteSpace(breakOption.Strength))
            {
                text += $" strength=\"{breakOption.Strength}\"";
            }

            text += " />";

            SsmlTextEditor.Document.Insert(offset, text);
        }
    }
}
