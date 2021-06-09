namespace SSMLEditor.Views
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using Catel.IoC;
    using Catel.MVVM;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.Rendering;
    using SSMLEditor.Analyzers;
    using SSMLEditor.AvalonEdit;
    using SSMLEditor.Services;
    using SSMLEditor.ViewModels;

    public partial class EditorView
    {
        private readonly IAnalyzerService _analyzerService;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isUpdatingFromSsmlEditor;
        private TextMarkerService _textMarkerService;

        public EditorView()
        {
            InitializeComponent();

            InitializeTextMarkerService();

            var serviceLocator = ServiceLocator.Default;
            _analyzerService = serviceLocator.ResolveType<IAnalyzerService>();
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

            var text = SsmlTextEditor.Text;

            (ViewModel as EditorViewModel)?.MarkSsmlDocumentAsChanged(text);

            BeginDocumentAnalyzer(text);

            _isUpdatingFromSsmlEditor = false;
        }

        private async void BeginDocumentAnalyzer(string documentText)
        {
            var currentCancellationTokenSource = _cancellationTokenSource;
            if (currentCancellationTokenSource is not null)
            {
                currentCancellationTokenSource.Cancel();
            }

            var newCancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource = newCancellationTokenSource;

            // TODO: optimize replacing the validation
            _textMarkerService.RemoveAll(x => true);

            var newAnalyzerResults = new List<AnalyzerResult>();

            await foreach (var analyzerResult in _analyzerService.AnalyzeAsync(documentText, newCancellationTokenSource.Token))
            {
                newAnalyzerResults.Add(analyzerResult);
            }

            if (!newCancellationTokenSource.IsCancellationRequested)
            {
                foreach (var analyzerResult in newAnalyzerResults)
                {
                    var marker = _textMarkerService.Create(analyzerResult.StartIndex, analyzerResult.Length);

                    marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                    marker.Tag = analyzerResult;

                    switch (analyzerResult.ResultType)
                    {
                        case AnalyzerResultType.Error:
                            marker.MarkerColor = Colors.Red;
                            break;

                        case AnalyzerResultType.Warning:
                            marker.MarkerColor = Colors.Orange;
                            break;
                    }
                }
            }
        }

        private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var relativeMousePosition = e.GetPosition(SsmlTextEditor);
            var mousePosition = SsmlTextEditor.GetPositionFromPoint(relativeMousePosition);
            if (mousePosition is null)
            {
                return;
            }

            var line = mousePosition.Value.Line;
            var column = mousePosition.Value.Column;
            var offset = SsmlTextEditor.Document.GetOffset(line, column);

            var toolTipLines = new List<string>();

            var markers = _textMarkerService.GetMarkersAtOffset(offset);
            if (markers.Any())
            {
                foreach (var marker in markers)
                {
                    if (marker.Tag is AnalyzerResult analyzerResult)
                    {
                        toolTipLines.Add(analyzerResult.Description);
                    }
                }
            }

            var toolTip = EditorToolTip;

            var showToolTip = toolTipLines.Count > 0;
            if (showToolTip)
            {
                toolTip.Content = string.Join("\r\n", toolTipLines);
                toolTip.SetCurrentValue(System.Windows.Controls.ToolTip.HorizontalOffsetProperty, 10d);
                toolTip.SetCurrentValue(System.Windows.Controls.ToolTip.VerticalOffsetProperty, 10d);
                toolTip.SetCurrentValue(System.Windows.Controls.ToolTip.PlacementProperty, PlacementMode.Mouse);
                toolTip.SetCurrentValue(System.Windows.Controls.ToolTip.PlacementTargetProperty, SsmlTextEditor);
            }

            toolTip.SetCurrentValue(System.Windows.Controls.ToolTip.IsOpenProperty, showToolTip);
        }

        protected override void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.HasPropertyChanged(nameof(EditorViewModel.SsmlDocument)) &&
                !_isUpdatingFromSsmlEditor)
            {
                SsmlTextEditor.Text = ((EditorViewModel)ViewModel).SsmlDocument;
            }
        }

        private void InitializeTextMarkerService()
        {
            var textMarkerService = new TextMarkerService(SsmlTextEditor.Document);
            SsmlTextEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            SsmlTextEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);

            var services = (IServiceContainer)SsmlTextEditor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services is not null)
            {
                services.AddService(typeof(ITextMarkerService), textMarkerService);
            }

            _textMarkerService = textMarkerService;
        }

        private bool IsSelected(ITextMarker marker)
        {
            int selectionEndOffset = SsmlTextEditor.SelectionStart + SsmlTextEditor.SelectionLength;
            if (marker.StartOffset >= SsmlTextEditor.SelectionStart && marker.StartOffset <= selectionEndOffset)
            {
                return true;
            }

            if (marker.EndOffset >= SsmlTextEditor.SelectionStart && marker.EndOffset <= selectionEndOffset)
            {
                return true;
            }

            return false;
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
