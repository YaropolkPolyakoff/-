using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace DanceApp.Views
{
    public partial class Demo3DView : UserControl
    {
        // ── Camera orbit state ────────────────────────────────────────────────
        private bool   _isDragging;
        private Point  _lastMousePos;
        private double _cameraAzimuth   = 0;    // degrees around Y
        private double _cameraElevation = 15;   // degrees above horizon
        private double _cameraDistance  = 6;

        // Default camera values
        private const double DefaultAzimuth   = 0;
        private const double DefaultElevation = 15;
        private const double DefaultDistance  = 6;

        private readonly List<string> _poses = new()
        {
            "1. Стойка — исходное положение",
            "2. Bounce — пружина",
            "3. Шаг вправо + наклон",
            "4. Шаг влево + наклон",
            "5. Freeze — стоп-кадр",
            "6. Вращение на 360°",
        };

        // Tilt / rotation angles per pose index
        private readonly double[] _poseAngles = { 0, 10, -15, 15, 0, 180 };

        public Demo3DView()
        {
            InitializeComponent();
            PoseButtonsControl.ItemsSource = _poses;
            UpdateCameraPosition();
        }

        // ── Camera positioning ────────────────────────────────────────────────
        private void UpdateCameraPosition()
        {
            var azRad  = _cameraAzimuth   * Math.PI / 180.0;
            var elRad  = _cameraElevation * Math.PI / 180.0;

            var x = _cameraDistance * Math.Sin(azRad) * Math.Cos(elRad);
            var y = _cameraDistance * Math.Sin(elRad);
            var z = _cameraDistance * Math.Cos(azRad) * Math.Cos(elRad);

            Camera.Position      = new Point3D(x, y + 1.0, z);
            Camera.LookDirection = new Vector3D(-x, -(y + 0.3), -z);
        }

        // ── Mouse orbit (360° drag) ────────────────────────────────────────────
        private void Viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging   = true;
            _lastMousePos = e.GetPosition(Viewport);
            Viewport.CaptureMouse();
        }

        private void Viewport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            Viewport.ReleaseMouseCapture();
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var currentPos = e.GetPosition(Viewport);
            var deltaX     = currentPos.X - _lastMousePos.X;
            var deltaY     = currentPos.Y - _lastMousePos.Y;

            _cameraAzimuth   += deltaX * 0.4;
            _cameraElevation -= deltaY * 0.3;

            // Clamp elevation to avoid gimbal flip
            _cameraElevation = Math.Max(-85, Math.Min(85, _cameraElevation));

            // Keep azimuth in [0, 360)
            _cameraAzimuth = (_cameraAzimuth % 360 + 360) % 360;

            _lastMousePos = currentPos;
            UpdateCameraPosition();
        }

        // ── Scroll = zoom ─────────────────────────────────────────────────────
        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _cameraDistance -= e.Delta / 120.0 * 0.3;
            _cameraDistance  = Math.Max(1.5, Math.Min(14, _cameraDistance));
            UpdateCameraPosition();
        }

        // ── Reset camera ──────────────────────────────────────────────────────
        private void ResetCamera_Click(object sender, RoutedEventArgs e)
        {
            _cameraAzimuth   = DefaultAzimuth;
            _cameraElevation = DefaultElevation;
            _cameraDistance  = DefaultDistance;
            UpdateCameraPosition();
        }

        // ── Auto-rotate toggle ────────────────────────────────────────────────
        private void AutoRotateToggle_Checked(object sender, RoutedEventArgs e)
        {
            var sb = (Storyboard)FindResource("AutoRotateStoryboard");
            sb.Begin(this, true);
        }

        private void AutoRotateToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            var sb = (Storyboard)FindResource("AutoRotateStoryboard");
            sb.Stop(this);
        }

        // ── Pose buttons ──────────────────────────────────────────────────────
        private void PoseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var label = btn.Content?.ToString() ?? string.Empty;
                int idx   = _poses.IndexOf(label);
                if (idx >= 0 && idx < _poseAngles.Length)
                    PlayPoseAnimation(idx);
            }
        }

        private void PlayPoseAnimation(int poseIndex)
        {
            double targetAngle = _poseAngles[poseIndex];

            if (poseIndex == 5)
            {
                // Full 360° spin around Y axis
                var currentAngle = DancerRotation.Angle;
                var anim360 = new DoubleAnimation(
                    currentAngle, currentAngle + 360,
                    TimeSpan.FromMilliseconds(900))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                };
                DancerRotation.BeginAnimation(
                    AxisAngleRotation3D.AngleProperty, anim360);
            }
            else
            {
                // Tilt on Z axis for the given pose
                var tiltAnim = new DoubleAnimation(targetAngle, TimeSpan.FromMilliseconds(400))
                {
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                    AutoReverse    = poseIndex != 0,
                    RepeatBehavior = poseIndex == 1
                        ? new RepeatBehavior(3)
                        : new RepeatBehavior(1)
                };
                DancerTiltRotation.BeginAnimation(
                    AxisAngleRotation3D.AngleProperty, tiltAnim);
            }
        }
    }
}
