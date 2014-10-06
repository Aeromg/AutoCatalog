using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using AutoCatalogLib.Business;

namespace AutoCatalogWpf.Utils
{
    public class RuleIdentificatorValidationRule : ValidationRule
    {
        private const string RuleNotExistMessage = @"Правило с таким идентификатором не существует";
        private const string RuleExistMessage = @"Правило с таким идентификатором уже существует";

        private const string WrongIdMessage =
            @"Неверно указан идентификатор. Допускается использование только латинских букв в нижнем регистре и цифр.";

        private static readonly Regex IdentificatorPattern = new Regex(@"^[a-z0-9]+$");
        public bool MustExists { get; set; }
        public bool DoNotCheckExists { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var str = value as string;
            if (String.IsNullOrEmpty(str) || !IdentificatorPattern.IsMatch(str))
                return new ValidationResult(false, WrongIdMessage);

            if (!DoNotCheckExists)
            {
                var rule = ImportRulesLocator.SearchBehavior(str);

                if (rule == null && MustExists)
                    return new ValidationResult(false, RuleNotExistMessage);

                if (rule != null && !MustExists)
                    return new ValidationResult(false, RuleExistMessage);
            }
            return new ValidationResult(true, @"");
        }
    }
}