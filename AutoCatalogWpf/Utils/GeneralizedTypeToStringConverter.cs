using System;
using System.Globalization;
using System.Windows.Data;
using AutoCatalogLib.Exchange;

namespace AutoCatalogWpf.Utils
{
    public class GeneralizedTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (GeneralizedType) value;

            switch (type)
            {
                case GeneralizedType.ArrayOfBoolean:
                    return @"Список логических значений";

                case GeneralizedType.ArrayOfDateTime:
                    return @"Список дат";

                case GeneralizedType.ArrayOfFloat:
                    return @"Список вещественных чисел";

                case GeneralizedType.ArrayOfInteger:
                    return @"Список целых чисел";

                case GeneralizedType.ArrayOfString:
                    return @"Список строк";

                case GeneralizedType.ArrayOfUnknown:
                    return @"Список объектов";

                case GeneralizedType.Boolean:
                    return @"Логическое значение";

                case GeneralizedType.DateTime:
                    return @"Дата";

                case GeneralizedType.Float:
                    return @"Вещественное число";

                case GeneralizedType.Integer:
                    return @"Целое число";

                case GeneralizedType.String:
                    return @"Строка";

                case GeneralizedType.Unknown:
                    return @"Объект";

                default:
                    return String.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}