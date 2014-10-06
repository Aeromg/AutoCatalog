using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using AutoCatalogLib.Exchange;

namespace AutoCatalogWpf.Utils
{
    public class ImportRuleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var importRule = value as IRule;
            if (value == null)
                return null;

            return String.Format(@"{0}: {1}", importRule.Identificator, importRule.Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
