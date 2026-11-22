using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using DanceApp.Views;

namespace DanceApp
{
    public partial class MainWindow : Window
    {
        private readonly CatalogView     _catalogView  = new();
        private readonly VideoPlayerView _playerView   = new();
        private readonly Demo3DView      _demo3dView   = new();

        public MainWindow()
        {
            InitializeComponent();
            NavigateTo(_catalogView);
        }

        // ── Navigation ──────────────────────────────────────────────────────
        private void NavigateTo(UIElement page)
        {
            PageHost.Content = page;
            var sb = (Storyboard)FindResource("PageFadeIn");
            sb.Begin(this);
        }

        private void NavCatalog_Click(object sender, RoutedEventArgs e)  => NavigateTo(_catalogView);
        private void NavPlayer_Click(object sender, RoutedEventArgs e)   => NavigateTo(_playerView);
        private void NavDemo3D_Click(object sender, RoutedEventArgs e)   => NavigateTo(_demo3dView);

        // Called by catalog cards to open the player for a specific style
        public void OpenPlayerForStyle(Models.DanceStyle style)
        {
            _playerView.LoadStyle(style);
            NavPlayer.IsChecked = true;
            NavigateTo(_playerView);
        }

        // ── Custom Title Bar ─────────────────────────────────────────────────
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                WindowState = WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            else
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}
