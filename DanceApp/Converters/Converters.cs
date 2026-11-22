using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DanceApp.Converters
{
    /// <summary>
    /// Converts a TimeSpan (or double seconds) to "mm:ss" string.
    /// </summary>
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
                return ts.ToString(@"mm\:ss");
            if (value is double d)
                return TimeSpan.FromSeconds(d).ToString(@"mm\:ss");
            return "00:00";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }

    /// <summary>
    /// Converts a boolean to Visibility (true → Visible, false → Collapsed).
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Visibility.Visible;
    }

    /// <summary>
    /// Inverted BoolToVisibility (true → Collapsed).
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true ? Visibility.Collapsed : Visibility.Visible;
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value is Visibility.Collapsed;
    }

    /// <summary>
    /// Converts hex color string to SolidColorBrush.
    /// </summary>
    public class HexColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is string hex)
                {
                    var color = (Color)ColorConverter.ConvertFromString(hex);
                    return new SolidColorBrush(color);
                }
            }
            catch { /* fall through */ }
            return Brushes.Transparent;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }

    /// <summary>
    /// Calculates the fill width for a custom slider track.
    /// Parameters: Value, Maximum, ActualWidth
    /// </summary>
    public class SliderFillWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3) return 0.0;
            if (values[0] is double val && values[1] is double max && values[2] is double width)
            {
                if (max <= 0) return 0.0;
                return Math.Max(0, Math.Min(width, val / max * width));
            }
            return 0.0;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => Array.Empty<object>();
    }

    /// <summary>
    /// Returns true if the step is active (highlighted in step list).
    /// </summary>
    public class ActiveStepToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6C3DE3"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1E1E32"));
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => DependencyProperty.UnsetValue;
    }
}
