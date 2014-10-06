using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace AutoCatalogWpf.Utils
{
    public class StringValidationRule : ValidationRule
    {
        private const string MustBeNotEmptyMessage = @"Значение не может быть пустым";

        private const string MustBeSimpleMessage =
            @"Значение может содержать только буквы латинского алфавита в нижем регистре и цифры";

        public static readonly Regex LowerLatinAndNumberPattern = new Regex(@"^[a-z0-9]+$");

        public bool NotEmpty { get; set; }
        public bool MustBeSimpleId { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;

            if (String.IsNullOrEmpty(str))
                return (NotEmpty || MustBeSimpleId)
                    ? new ValidationResult(false, MustBeNotEmptyMessage)
                    : new ValidationResult(true, @"");

            if (MustBeSimpleId && !LowerLatinAndNumberPattern.IsMatch(str))
                return new ValidationResult(false, MustBeSimpleMessage);

            return new ValidationResult(true, @"");
        }
    }
}