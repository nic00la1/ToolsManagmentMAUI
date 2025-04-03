using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace ToolsManagmentMAUI.Converters
{
    public class QuantityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantity && quantity == 0)
            {
                return Colors.Red;
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}