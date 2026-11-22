using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DanceApp.Models;

namespace DanceApp.Views
{
    public partial class CatalogView : UserControl
    {
        public CatalogView()
        {
            InitializeComponent();
            StylesItemsControl.ItemsSource = DanceData.GetStyles();
        }

        /// <summary>
        /// Mouse click on a card — opens the video player for that style.
        /// </summary>
        private void CardBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is DanceStyle style)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.OpenPlayerForStyle(style);
            }
        }
    }
}
