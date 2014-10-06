using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace AutoCatalogWpf.Utils
{
    public class NumbersValidationRule : ValidationRule
    {
        private const string NotANumberMessage = @"Значение не является числом";
        private const string EmptyValueMessage = @"Значение не может быть пустым";
        private const string MustBeIntegerMessage = @"Значение должно быть целым числом";
        private const string MustBeNotNegativeMessage = @"Значение должно быть больше или равно нулю";
        private const string MustBePositiveIntegerMessage = @"Значение должно быть целым числом больше нуля";

        private static readonly Regex DecimalPattern = new Regex(@"^\-?\d+([,.]\d+)?$");
        private static readonly Regex IntegerPattern = new Regex(@"^\-?\d+$");
        private static readonly Regex PositiveIntegerPattern = new Regex(@"^\d+$");

        public bool MustBeInteger { get; set; }
        public bool MustBePositive { get; set; }
        public bool MustBeNotNegative { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, EmptyValueMessage);

            var str = value.ToString();

            if (!DecimalPattern.IsMatch(str))
                return new ValidationResult(false, NotANumberMessage);

            if (MustBeInteger && !IntegerPattern.IsMatch(str))
                return new ValidationResult(false, MustBeIntegerMessage);

            if (MustBePositive)
                if (!PositiveIntegerPattern.IsMatch(str))
                    return new ValidationResult(false, MustBePositiveIntegerMessage);
                else
                    try
                    {
                        if (((int) Convert.ChangeType(value, typeof (Int32))) <= 0)
                            return new ValidationResult(false, MustBePositiveIntegerMessage);
                    }
                    catch (Exception)
                    {
                        return new ValidationResult(false, MustBePositiveIntegerMessage);
                    }

            if (MustBeNotNegative)
                try
                {
                    if (((int) Convert.ChangeType(value, typeof (Int32))) < 0)
                        return new ValidationResult(false, MustBeNotNegativeMessage);
                }
                catch (Exception)
                {
                    return new ValidationResult(false, MustBePositiveIntegerMessage);
                }

            return new ValidationResult(true, String.Empty);
        }
    }
}