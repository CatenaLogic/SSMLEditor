namespace SSMLEditor.AvalonEdit
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using Catel;
    using ICSharpCode.AvalonEdit.Document;

    public sealed class TextMarker : TextSegment, ITextMarker
    {
        private readonly TextMarkerService _service;

        public TextMarker(TextMarkerService service, int startOffset, int length)
        {
            ArgumentNullException.ThrowIfNull(service);

            _service = service;
            StartOffset = startOffset;
            Length = length;
            _markerTypes = TextMarkerTypes.None;
        }

        public event EventHandler Deleted;

        public bool IsDeleted
        {
            get { return !IsConnectedToCollection; }
        }

        public void Delete()
        {
            _service.Remove(this);
        }

        internal void OnDeleted()
        {
            if (Deleted is not null)
                Deleted(this, EventArgs.Empty);
        }

        private void Redraw()
        {
            _service.Redraw(this);
        }

        private Color? _backgroundColor;

        public Color? BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    Redraw();
                }
            }
        }

        private Color? _foregroundColor;

        public Color? ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                if (_foregroundColor != value)
                {
                    _foregroundColor = value;
                    Redraw();
                }
            }
        }

        private FontWeight? _fontWeight;

        public FontWeight? FontWeight
        {
            get { return _fontWeight; }
            set
            {
                if (_fontWeight != value)
                {
                    _fontWeight = value;
                    Redraw();
                }
            }
        }

        private FontStyle? _fontStyle;

        public FontStyle? FontStyle
        {
            get { return _fontStyle; }
            set
            {
                if (_fontStyle != value)
                {
                    _fontStyle = value;
                    Redraw();
                }
            }
        }

        public object Tag { get; set; }

        private TextMarkerTypes _markerTypes;

        public TextMarkerTypes MarkerTypes
        {
            get { return _markerTypes; }
            set
            {
                if (_markerTypes != value)
                {
                    _markerTypes = value;
                    Redraw();
                }
            }
        }

        private Color _markerColor;

        public Color MarkerColor
        {
            get { return _markerColor; }
            set
            {
                if (_markerColor != value)
                {
                    _markerColor = value;
                    Redraw();
                }
            }
        }

        public object ToolTip { get; set; }
    }
}
