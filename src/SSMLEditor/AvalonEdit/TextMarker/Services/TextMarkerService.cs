namespace SSMLEditor.AvalonEdit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Rendering;

    public sealed class TextMarkerService : DocumentColorizingTransformer, IBackgroundRenderer, ITextMarkerService, ITextViewConnect
    {
        private readonly TextSegmentCollection<TextMarker> _markers;
        private readonly TextDocument _document;

        public TextMarkerService(TextDocument document)
        {
            ArgumentNullException.ThrowIfNull(document);

            _document = document;
            _markers = new TextSegmentCollection<TextMarker>(document);
        }

        #region ITextMarkerService
        public ITextMarker Create(int startOffset, int length)
        {
            if (_markers is null)
                throw new InvalidOperationException("Cannot create a marker when not attached to a document");

            int textLength = _document.TextLength;
            if (startOffset < 0 || startOffset > textLength)
                throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between 0 and " + textLength);
            if (length < 0 || startOffset + length > textLength)
                throw new ArgumentOutOfRangeException("length", length, "length must not be negative and startOffset+length must not be after the end of the document");

            var m = new TextMarker(this, startOffset, length);

            _markers.Add(m);
            // no need to mark segment for redraw: the text marker is invisible until a property is set

            return m;
        }

        public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
        {
            if (_markers is null)
            {
                return Enumerable.Empty<ITextMarker>();
            }
            else
            {
                return _markers.FindSegmentsContaining(offset);
            }
        }

        public IEnumerable<ITextMarker> TextMarkers
        {
            get { return _markers ?? Enumerable.Empty<ITextMarker>(); }
        }

        public void RemoveAll(Predicate<ITextMarker> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            if (_markers is not null)
            {
                foreach (var m in _markers.ToArray())
                {
                    if (predicate(m))
                    {
                        Remove(m);
                    }
                }
            }
        }

        public void Remove(ITextMarker marker)
        {
            ArgumentNullException.ThrowIfNull(marker);

            var m = marker as TextMarker;

            if (_markers is not null && _markers.Remove(m))
            {
                Redraw(m);
                m.OnDeleted();
            }
        }

        /// <summary>
        /// Redraws the specified text segment.
        /// </summary>
        internal void Redraw(ISegment segment)
        {
            foreach (var view in _textViews)
            {
                view.Redraw(segment, DispatcherPriority.Normal);
            }

            RedrawRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler RedrawRequested;
        #endregion

        #region DocumentColorizingTransformer
        protected override void ColorizeLine(DocumentLine line)
        {
            if (_markers is null)
            {
                return;
            }

            int lineStart = line.Offset;
            int lineEnd = lineStart + line.Length;

            foreach (var marker in _markers.FindOverlappingSegments(lineStart, line.Length))
            {
                Brush foregroundBrush = null;
                if (marker.ForegroundColor is not null)
                {
                    foregroundBrush = new SolidColorBrush(marker.ForegroundColor.Value);
                    foregroundBrush.Freeze();
                }

                ChangeLinePart(
                    Math.Max(marker.StartOffset, lineStart),
                    Math.Min(marker.EndOffset, lineEnd),
                    element =>
                    {
                        if (foregroundBrush is not null)
                        {
                            element.TextRunProperties.SetForegroundBrush(foregroundBrush);
                        }

                        var tf = element.TextRunProperties.Typeface;
                        element.TextRunProperties.SetTypeface(new Typeface(
                            tf.FontFamily,
                            marker.FontStyle ?? tf.Style,
                            marker.FontWeight ?? tf.Weight,
                            tf.Stretch
                        ));
                    }
                );
            }
        }
        #endregion

        #region IBackgroundRenderer
        public KnownLayer Layer
        {
            get
            {
                // draw behind selection
                return KnownLayer.Selection;
            }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            ArgumentNullException.ThrowIfNull(textView);
            ArgumentNullException.ThrowIfNull(drawingContext);

            if (_markers is null || !textView.VisualLinesValid)
            {
                return;
            }

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
            {
                return;
            }

            var viewStart = visualLines.First().FirstDocumentLine.Offset;
            var viewEnd = visualLines.Last().LastDocumentLine.EndOffset;

            foreach (var marker in _markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
            {
                if (marker.BackgroundColor is not null)
                {
                    var geoBuilder = new BackgroundGeometryBuilder();
                    geoBuilder.AlignToWholePixels = true;
                    geoBuilder.CornerRadius = 3;
                    geoBuilder.AddSegment(textView, marker);
                    var geometry = geoBuilder.CreateGeometry();
                    if (geometry is not null)
                    {
                        var color = marker.BackgroundColor.Value;
                        var brush = new SolidColorBrush(color);
                        brush.Freeze();
                        drawingContext.DrawGeometry(brush, null, geometry);
                    }
                }

                var underlineMarkerTypes = TextMarkerTypes.SquigglyUnderline | TextMarkerTypes.NormalUnderline | TextMarkerTypes.DottedUnderline;
                if ((marker.MarkerTypes & underlineMarkerTypes) != 0)
                {
                    foreach (var r in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
                    {
                        var startPoint = r.BottomLeft;
                        var endPoint = r.BottomRight;

                        var usedBrush = new SolidColorBrush(marker.MarkerColor);
                        usedBrush.Freeze();

                        if ((marker.MarkerTypes & TextMarkerTypes.SquigglyUnderline) != 0)
                        {
                            var offset = 2.5;

                            var count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);

                            var geometry = new StreamGeometry();

                            using (var ctx = geometry.Open())
                            {
                                ctx.BeginFigure(startPoint, false, false);
                                ctx.PolyLineTo(CreatePoints(startPoint, endPoint, offset, count).ToArray(), true, false);
                            }

                            geometry.Freeze();

                            var usedPen = new Pen(usedBrush, 1);
                            usedPen.Freeze();
                            drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
                        }

                        if ((marker.MarkerTypes & TextMarkerTypes.NormalUnderline) != 0)
                        {
                            var usedPen = new Pen(usedBrush, 1);
                            usedPen.Freeze();
                            drawingContext.DrawLine(usedPen, startPoint, endPoint);
                        }

                        if ((marker.MarkerTypes & TextMarkerTypes.DottedUnderline) != 0)
                        {
                            var usedPen = new Pen(usedBrush, 1);
                            usedPen.DashStyle = DashStyles.Dot;
                            usedPen.Freeze();
                            drawingContext.DrawLine(usedPen, startPoint, endPoint);
                        }
                    }
                }
            }
        }

        private IEnumerable<Point> CreatePoints(Point start, Point end, double offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Point(start.X + i * offset, start.Y - ((i + 1) % 2 == 0 ? offset : 0));
            }
        }
        #endregion

        #region ITextViewConnect
        private readonly List<TextView> _textViews = new List<TextView>();

        void ITextViewConnect.AddToTextView(TextView textView)
        {
            if (textView is not null && !_textViews.Contains(textView))
            {
                Debug.Assert(textView.Document == _document);

                _textViews.Add(textView);
            }
        }

        void ITextViewConnect.RemoveFromTextView(TextView textView)
        {
            if (textView is not null)
            {
                Debug.Assert(textView.Document == _document);

                _textViews.Remove(textView);
            }
        }
        #endregion
    }
}
