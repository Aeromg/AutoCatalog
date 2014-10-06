using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutoCatalogWpf.Utils
{
    public class FilePathValidationRule : ValidationRule
    {
        public bool MustBeNotEmpty { get; set; }

        private static readonly Regex Pattern = new Regex(@"^[a-z]:\\([\w \+\!\.\,\-\=\(\)]+\\)*([\w \+\!\.\,\-\=\(\)\*\?]*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private const string MustBeNotEmptyMessage = @"Значение не может быть пустым";
        private const string InvalidPath = @"Значение не является путем к файлу, каталогу или маской файла";

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var path = value as String;
            if (String.IsNullOrEmpty(path))
                return MustBeNotEmpty
                    ? new ValidationResult(false, MustBeNotEmptyMessage)
                    : new ValidationResult(true, @"");

            if(!Pattern.IsMatch(path))
                return new ValidationResult(false, InvalidPath);

            return new ValidationResult(true, @"");
        }
    }
}
