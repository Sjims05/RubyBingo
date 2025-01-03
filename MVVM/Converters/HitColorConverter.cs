using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RubyBingoApp.MVVM.Converters
{
    public class HitColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int hitValue = (int)value;

            switch (hitValue)
            {
                case 0:
                    return Application.Current.Resources["Hit-Yellow-Border"];
                case 1:
                    return Application.Current.Resources["Hit-Blue-Border"];
                case 2:
                    return Application.Current.Resources["Hit-White-Border"];
                case 3:
                    return Application.Current.Resources["Hit-Grey-Border"];
                default:
                    return Application.Current.Resources["Hit-Border"]; // default style if value isn't recognized
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
