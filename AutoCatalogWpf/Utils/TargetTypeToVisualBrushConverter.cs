using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using AutoCatalogLib.Exchange.ImportTargets;

namespace AutoCatalogWpf.Utils
{
    public class TargetTypeToVisualBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is TargetType))
                return null;

            switch ((TargetType)value)
            {
                case TargetType.File:
                    return Application.Current.Resources["appbar_folder"];

                case TargetType.Web:
                    return Application.Current.Resources["appbar_browser"];

                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
