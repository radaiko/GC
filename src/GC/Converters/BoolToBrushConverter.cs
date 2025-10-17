using System;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace GC.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool selected = false;
        if (value is bool b) selected = b;

        // Parameter can be used to invert logic or pass color info in future
        return selected ? new SolidColorBrush(Color.Parse("#007AFF")) : new SolidColorBrush(Color.Parse("#8E8E93"));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

