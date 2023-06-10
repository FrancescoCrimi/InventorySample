using Microsoft.Extensions.Logging;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Inventory.Uwp.Library.Converters
{
    public sealed class LogLevelConverter : IValueConverter
    {
        private readonly SolidColorBrush TraceColor = new SolidColorBrush(Colors.DarkGray);
        private readonly SolidColorBrush DebugColor = new SolidColorBrush(Colors.Gray);
        private readonly SolidColorBrush InformationColor = new SolidColorBrush(Colors.Navy);
        private readonly SolidColorBrush WarningColor = new SolidColorBrush(Colors.Magenta);
        private readonly SolidColorBrush ErrorColor = new SolidColorBrush(Colors.Yellow);
        private readonly SolidColorBrush CriticalColor = new SolidColorBrush(Colors.Red);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == typeof(string))
            {
                if (value is LogLevel logLevel)
                {
                    switch (logLevel)
                    {
                        case LogLevel.Trace:
                            return Char.ConvertFromUtf32(0xEADF).ToString();
                        case LogLevel.Debug:
                            return Char.ConvertFromUtf32(0xEBE8).ToString();
                        case LogLevel.Information:
                            return Char.ConvertFromUtf32(0xE946).ToString();
                        case LogLevel.Warning:
                            return Char.ConvertFromUtf32(0xE7BA).ToString();
                        case LogLevel.Error:
                            return Char.ConvertFromUtf32(0xEA39).ToString();
                        case LogLevel.Critical:
                            return Char.ConvertFromUtf32(0xE783).ToString();
                        case LogLevel.None:
                        default:
                            return Char.ConvertFromUtf32(0xE946).ToString();
                    }
                }
            }

            if (targetType == typeof(Brush))
            {
                if (value is LogLevel logType)
                {
                    switch (logType)
                    {
                        case LogLevel.Trace:
                            return TraceColor;
                        case LogLevel.Debug:
                            return DebugColor;
                        case LogLevel.Information:
                            return InformationColor;
                        case LogLevel.Warning:
                            return WarningColor;
                        case LogLevel.Error:
                            return ErrorColor;
                        case LogLevel.Critical:
                            return CriticalColor;
                        case LogLevel.None:
                        default:
                            return InformationColor;
                    }
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
