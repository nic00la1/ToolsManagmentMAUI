using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace ToolsManagmentMAUI.Converters;

public class NullToDefaultImageConverter : IValueConverter
{
    public object Convert(object value,
                          Type targetType,
                          object parameter,
                          CultureInfo culture
    )
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return ImageSource.FromFile(parameter.ToString());
        return ImageSource.FromUri(new Uri(value.ToString()));
    }

    public object ConvertBack(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture
    )
    {
        throw new NotImplementedException();
    }
}
