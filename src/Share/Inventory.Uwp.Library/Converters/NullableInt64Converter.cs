using System;
using Windows.UI.Xaml.Data;

namespace Inventory.Uwp.Library.Converters
{
    public sealed class NullableInt64Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Int64 n64)
            {
                return n64;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                if (Int64.TryParse(value.ToString(), out Int64 n64))
                {
                    return n64;
                }
            }
            return null;
        }
    }
}
