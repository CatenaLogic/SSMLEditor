namespace SSMLEditor.Views
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Threading;
    using Catel.Collections;
    using SSMLEditor.ViewModels;

    public partial class VideoView
    {
        private readonly DispatcherTimer _positionDispatcherTimer;
        private readonly DispatcherTimer _positionUpdateDispatcherTimer;

        private bool _isUserUpdatingSlider;
        private bool _isAppUpdatingSlider;

        public VideoView()
        {
            InitializeComponent();

            _positionDispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };

            _positionUpdateDispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
        }

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);

            _positionDispatcherTimer.Tick += OnPositionDispatcherTimerTick;
            _positionUpdateDispatcherTimer.Tick += OnPositionUpdateDispatcherTimerTick;
        }

        protected override void OnUnloaded(EventArgs e)
        {
            _positionDispatcherTimer.Tick -= OnPositionDispatcherTimerTick;
            _positionUpdateDispatcherTimer.Tick -= OnPositionUpdateDispatcherTimerTick;

            base.OnUnloaded(e);
        }

        protected override void OnViewModelPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnViewModelPropertyChanged(e);

            if (e.HasPropertyChanged(nameof(VideoViewModel.VideoUri)))
            {
                var vm = (VideoViewModel)ViewModel;
                if (vm.VideoUri is null)
                {
                    ProgressSlider.SetCurrentValue(System.Windows.Controls.Primitives.RangeBase.MaximumProperty, 0d);
                }

                // Pause so the media actually gets loaded
                UpdateMediaElements(x => x.Pause());
            }
            else if (e.HasPropertyChanged(nameof(VideoViewModel.Position)))
            {
                if (_isAppUpdatingSlider)
                {
                    return;
                }

                var vm = (VideoViewModel)ViewModel;

                UpdateMediaElements(x => x.Position = vm.Position);

                if (!vm.IsPlaying)
                {
                    // Pause so the media actually gets loaded to the new position
                    UpdateMediaElements(x => x.Pause());
                }
            }
            else if (e.HasPropertyChanged(nameof(VideoViewModel.BaseAudioUri)))
            {
                var vm = (VideoViewModel)ViewModel;
                if (vm.IsPlaying && vm.BaseAudioUri is not null)
                {
                    // Ensure that base audio starts playing on the right position
                    BaseAudioMediaElement.Position = VideoMediaElement.Position;
                    BaseAudioMediaElement.Play();
                }
            }
            else if (e.HasPropertyChanged(nameof(VideoViewModel.IsPlaying)))
            {
                var vm = (VideoViewModel)ViewModel;
                if (vm.IsPlaying)
                {
                    _positionDispatcherTimer.Start();
                    UpdateMediaElements(x => x.Play());
                }
                else
                {
                    _positionDispatcherTimer.Stop();
                    UpdateMediaElements(x => x.Pause());
                }
            }
        }

        private void OnPositionDispatcherTimerTick(object sender, EventArgs e)
        {
            if (_isUserUpdatingSlider)
            {
                return;
            }

            var position = VideoMediaElement.Position;

            _isAppUpdatingSlider = true;

            ProgressSlider.SetCurrentValue(Slider.ValueProperty, position.TotalSeconds);

            _isAppUpdatingSlider = false;

            var vm = ViewModel as VideoViewModel;
            if (vm is not null)
            {
                vm.Position = position;
            }
        }

        private void OnPositionUpdateDispatcherTimerTick(object sender, EventArgs e)
        {
            _positionUpdateDispatcherTimer.Stop();

            _isUserUpdatingSlider = true;

            var vm = ViewModel as VideoViewModel;
            if (vm is not null)
            {
                vm.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
            }

            _isUserUpdatingSlider = false;
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _isUserUpdatingSlider = true;
        }

        private void OnProgressSliderValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isAppUpdatingSlider)
            {
                return;
            }

            _positionUpdateDispatcherTimer.Stop();
            _positionUpdateDispatcherTimer.Start();
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isUserUpdatingSlider = false;
        }

        private void OnMediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            var duration = VideoMediaElement.NaturalDuration.TimeSpan;

            ((VideoViewModel)ViewModel).TotalDuration = duration;
            ProgressSlider.SetCurrentValue(RangeBase.MaximumProperty, duration.TotalSeconds);
        }

        private void OnMediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {

        }

        private void OnMediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = ViewModel as VideoViewModel;
            if (vm is not null)
            {
                vm.Pause.Execute();
            }
        }

        private void UpdateMediaElements(Action<MediaElement> action)
        {
            var elements = new[]
            {
                VideoMediaElement,
                AudioMediaElement,
                BaseAudioMediaElement
            };

            elements.ForEach(x =>
            {
                if (x.Source is not null)
                {
                    action(x);
                }
            });
        }
    }
}
