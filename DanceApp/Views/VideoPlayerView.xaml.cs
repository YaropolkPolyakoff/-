using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DanceApp.Models;

namespace DanceApp.Views
{
    public partial class VideoPlayerView : UserControl
    {
        // ── State ────────────────────────────────────────────────────────────
        private bool   _isPlaying;
        private bool   _isSeeking;
        private bool   _isMirrored;
        private bool   _loopEnabled;
        private double _loopStart = -1;
        private double _loopEnd   = -1;
        private bool   _loopMarkerASet;
        private DanceLesson? _currentLesson;
        private List<DanceStep> _steps = new();

        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(250) };

        public VideoPlayerView()
        {
            InitializeComponent();
            _timer.Tick += Timer_Tick;
        }

        // ── Public API ───────────────────────────────────────────────────────
        public void LoadStyle(DanceStyle style)
        {
            StyleNameTb.Text = style.Name;
            LessonCombo.ItemsSource       = style.Lessons;
            LessonCombo.DisplayMemberPath = "Title";
            if (style.Lessons.Count > 0)
                LessonCombo.SelectedIndex = 0;
        }

        // ── Lesson selection ─────────────────────────────────────────────────
        private void LessonCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LessonCombo.SelectedItem is DanceLesson lesson)
                LoadLesson(lesson);
        }

        private void LoadLesson(DanceLesson lesson)
        {
            _currentLesson = lesson;
            LessonTitleTb.Text = lesson.Title;

            // Reset player state
            StopPlayback();
            ResetLoopMarkers();

            _steps = lesson.Steps.ToList();
            StepsListControl.ItemsSource = _steps;

            // Try loading the video
            try
            {
                VideoPlayer.Source = new Uri(lesson.VideoPath, UriKind.RelativeOrAbsolute);
                VideoPlayer.Play();
                VideoPlayer.Pause();   // open but don't auto-play
                PlaceholderPanel.Visibility = Visibility.Collapsed;
            }
            catch
            {
                PlaceholderPanel.Visibility = Visibility.Visible;
            }
        }

        // ── MediaElement events ───────────────────────────────────────────────
        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                var duration = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                SeekSlider.Maximum = duration;
                DurationLabel.Text = TimeSpan.FromSeconds(duration).ToString(@"mm\:ss");
            }
            PlaceholderPanel.Visibility = Visibility.Collapsed;
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            SetPlayingState(false);
            SeekSlider.Value = 0;
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            PlaceholderPanel.Visibility = Visibility.Visible;
        }

        // ── Timer ─────────────────────────────────────────────────────────────
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_isSeeking || VideoPlayer.Source == null) return;

            var pos = VideoPlayer.Position.TotalSeconds;

            // Update slider
            _isSeeking = true;
            SeekSlider.Value = pos;
            _isSeeking = false;

            PositionLabel.Text = TimeSpan.FromSeconds(pos).ToString(@"mm\:ss");

            // A-B loop check
            if (_loopEnabled && _loopStart >= 0 && _loopEnd > _loopStart && pos >= _loopEnd)
            {
                VideoPlayer.Position = TimeSpan.FromSeconds(_loopStart);
            }

            // Highlight active step
            foreach (var step in _steps)
                step.IsActive = pos >= step.StartTime && pos < step.EndTime;
            StepsListControl.Items.Refresh();
        }

        // ── Play / Pause ──────────────────────────────────────────────────────
        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                VideoPlayer.Pause();
                SetPlayingState(false);
            }
            else
            {
                VideoPlayer.Play();
                SetPlayingState(true);
            }
        }

        private void SetPlayingState(bool playing)
        {
            _isPlaying = playing;
            PlayIcon.Visibility  = playing ? Visibility.Collapsed : Visibility.Visible;
            PauseIcon.Visibility = playing ? Visibility.Visible   : Visibility.Collapsed;
            if (playing) _timer.Start(); else _timer.Stop();
        }

        private void StopPlayback()
        {
            VideoPlayer.Stop();
            SetPlayingState(false);
            SeekSlider.Value   = 0;
            PositionLabel.Text = "00:00";
        }

        // ── Skip ──────────────────────────────────────────────────────────────
        private void SkipBack_Click(object sender, RoutedEventArgs e)
        {
            var newPos = Math.Max(0, VideoPlayer.Position.TotalSeconds - 10);
            VideoPlayer.Position = TimeSpan.FromSeconds(newPos);
        }

        private void SkipForward_Click(object sender, RoutedEventArgs e)
        {
            if (!VideoPlayer.NaturalDuration.HasTimeSpan) return;
            var max    = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            var newPos = Math.Min(max, VideoPlayer.Position.TotalSeconds + 10);
            VideoPlayer.Position = TimeSpan.FromSeconds(newPos);
        }

        // ── Seek slider ───────────────────────────────────────────────────────
        private void SeekSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
            => _isSeeking = true;

        private void SeekSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isSeeking = false;
            VideoPlayer.Position = TimeSpan.FromSeconds(SeekSlider.Value);
        }

        private void SeekSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isSeeking)
                VideoPlayer.Position = TimeSpan.FromSeconds(SeekSlider.Value);
        }

        // ── Speed ─────────────────────────────────────────────────────────────
        private void SpeedButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag is string tagStr
                && double.TryParse(tagStr, System.Globalization.NumberStyles.Float,
                                   System.Globalization.CultureInfo.InvariantCulture, out double speed))
            {
                VideoPlayer.SpeedRatio = speed;
            }
        }

        // ── Mirror ────────────────────────────────────────────────────────────
        private void MirrorToggle_Checked(object sender, RoutedEventArgs e)
        {
            _isMirrored = true;
            AnimateMirror(-1);
        }

        private void MirrorToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _isMirrored = false;
            AnimateMirror(1);
        }

        private void AnimateMirror(double targetScaleX)
        {
            var anim = new DoubleAnimation(targetScaleX, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            MirrorTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
        }

        // ── A-B Loop ──────────────────────────────────────────────────────────
        private void LoopToggle_Checked(object sender, RoutedEventArgs e)
        {
            _loopEnabled    = true;
            _loopMarkerASet = false;
            _loopStart      = -1;
            _loopEnd        = -1;
            VideoPlayer.MouseLeftButtonDown += VideoPlayer_SetLoopMarker;
        }

        private void LoopToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            _loopEnabled = false;
            VideoPlayer.MouseLeftButtonDown -= VideoPlayer_SetLoopMarker;
            ResetLoopMarkers();
        }

        private void VideoPlayer_SetLoopMarker(object sender, MouseButtonEventArgs e)
        {
            if (!VideoPlayer.NaturalDuration.HasTimeSpan) return;
            var pos     = VideoPlayer.Position.TotalSeconds;
            var total   = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            var sliderW = SeekSlider.ActualWidth;

            if (!_loopMarkerASet)
            {
                _loopStart      = pos;
                _loopMarkerASet = true;
                MarkerA.SetValue(System.Windows.Controls.Canvas.LeftProperty, pos / total * sliderW);
                MarkerA.Visibility = Visibility.Visible;
            }
            else
            {
                _loopEnd        = Math.Max(pos, _loopStart + 1);
                _loopMarkerASet = false;
                MarkerB.SetValue(System.Windows.Controls.Canvas.LeftProperty, _loopEnd / total * sliderW);
                MarkerB.Visibility = Visibility.Visible;
            }
        }

        private void ResetLoopMarkers()
        {
            _loopStart = _loopEnd = -1;
            _loopMarkerASet = false;
            MarkerA.Visibility = MarkerB.Visibility = Visibility.Collapsed;
        }

        // ── Step click → seek ─────────────────────────────────────────────────
        private void StepItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is DanceStep step)
            {
                VideoPlayer.Position = TimeSpan.FromSeconds(step.StartTime);
                if (!_isPlaying)
                {
                    VideoPlayer.Play();
                    SetPlayingState(true);
                }
            }
        }
    }
}
